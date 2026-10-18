{ Win32 console replacement for the Turbo Pascal WIN unit (WIN.ASM).
  Direct writes go through the console API instead of video memory.
  Also hosts Sound/NoSound/Delay, which play through Windows' Beep. }

unit Win;

interface

uses Crt;

procedure WriteStr(X, Y: Byte; S: String; Attr: Byte);
procedure WriteChar(X, Y, Count: Byte; Ch: Char; Attr: Byte);

procedure Sound(Hz: Word);
procedure NoSound;
procedure Delay(Ms: Word);
function ReadKey: Char;

implementation

uses Windows;

type
  TIOFunc = function(var T: TextRec): Longint;

const
  { Crt's Write treats bytes as Windows-1252, the game's text is CP437. }
  To1252: array[0..255] of Byte = (
      0,   1,   2,   3,   4,   5,   6,   7,   8,   9,  10,  11,  12,  13,  14,  15,
     16,  17,  18,  19,  20,  21,  22,  23,  24,  25,  26,  27,  28,  29,  30,  31,
     32,  33,  34,  35,  36,  37,  38,  39,  40,  41,  42,  43,  44,  45,  46,  47,
     48,  49,  50,  51,  52,  53,  54,  55,  56,  57,  58,  59,  60,  61,  62,  63,
     64,  65,  66,  67,  68,  69,  70,  71,  72,  73,  74,  75,  76,  77,  78,  79,
     80,  81,  82,  83,  84,  85,  86,  87,  88,  89,  90,  91,  92,  93,  94,  95,
     96,  97,  98,  99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111,
    112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127,
    199, 252, 233, 226, 228, 224, 229, 231, 234, 235, 232, 239, 238, 236, 196, 197,
    201, 230, 198, 244, 246, 242, 251, 249, 255, 214, 220, 162, 163, 165,  63, 131,
    225, 237, 243, 250, 241, 209, 170, 186, 191,  63, 172, 189, 188, 161, 171, 187,
     35,  35,  35,  43,  43,  43,  43,  43,  43,  43,  43,  43,  43,  43,  43,  43,
     43,  43,  43,  43,  43,  43,  43,  43,  43,  43,  43,  43,  43,  43,  43,  43,
     43,  43,  43,  43,  43,  43,  43,  43,  43,  43,  43,  35,  35,  35,  35,  35,
     63, 223,  63,  63,  63,  63, 181,  63,  63,  63,  63,  63,  63,  63,  63,  63,
     63, 177,  63,  63,  63,  63, 247,  63, 176,  63, 183,  63,  63, 178,  63, 160);

  To437: array[0..255] of Byte = (
      0,   1,   2,   3,   4,   5,   6,   7,   8,   9,  10,  11,  12,  13,  14,  15,
     16,  17,  18,  19,  20,  21,  22,  23,  24,  25,  26,  27,  28,  29,  30,  31,
     32,  33,  34,  35,  36,  37,  38,  39,  40,  41,  42,  43,  44,  45,  46,  47,
     48,  49,  50,  51,  52,  53,  54,  55,  56,  57,  58,  59,  60,  61,  62,  63,
     64,  65,  66,  67,  68,  69,  70,  71,  72,  73,  74,  75,  76,  77,  78,  79,
     80,  81,  82,  83,  84,  85,  86,  87,  88,  89,  90,  91,  92,  93,  94,  95,
     96,  97,  98,  99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111,
    112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127,
     63,  63,  63, 159,  63,  63,  63,  63,  63,  63,  63,  63,  63,  63,  63,  63,
     63,  63,  63,  63,  63,  63,  63,  63,  63,  63,  63,  63,  63,  63,  63,  63,
    255, 173, 155, 156,  63, 157,  63,  63,  63,  63, 166, 174, 170,  63,  63,  63,
    248, 241, 253,  63,  63, 230,  63, 250,  63,  63, 167, 175, 172, 171,  63, 168,
     63,  63,  63,  63, 142, 143, 146, 128,  63, 144,  63,  63,  63,  63,  63,  63,
     63, 165,  63,  63,  63,  63, 153,  63,  63,  63,  63,  63, 154,  63,  63, 225,
    133, 160, 131,  63, 132, 134, 145, 135, 138, 130, 136, 137, 141, 161, 140, 139,
     63, 164, 149, 162, 147,  63, 148, 246,  63, 151, 163, 150, 129,  63,  63, 152);

var
  Out      : THandle;
  Pending  : Word;
  OldWrite : TIOFunc;
  Extended : Boolean;

function NewWrite(var T: TextRec): Longint;
var
  i: Cardinal;
begin
  for i := 0 to T.BufPos - 1 do
    T.BufPtr^[i] := Chr(To1252[Ord(T.BufPtr^[i])]);
  NewWrite := OldWrite(T);
end;

function ReadKey: Char;
var
  c: Char;
begin
  c := Crt.ReadKey;
  if Extended
    then Extended := false
    else
      if c = #0
        then Extended := true
        else c := Chr(To437[Ord(c)]);
  ReadKey := c;
end;

function Origin(X, Y: Byte): Windows.COORD;
begin
  Origin.X := Lo(WindMin) + X - 1;
  Origin.Y := Hi(WindMin) + Y - 1;
end;

function Room(X: Byte): Integer;
var
  Info: TConsoleScreenBufferInfo;
begin
  GetConsoleScreenBufferInfo(Out, Info);
  Room := Info.dwSize.X - (Lo(WindMin) + X - 1);
end;

procedure WriteStr(X, Y: Byte; S: String; Attr: Byte);
var
  Attrs   : array[0..255] of Word;
  Written : DWORD;
  i, n    : Integer;
begin
  n := Length(S);
  if Room(X) < n then n := Room(X);
  if n <= 0 then Exit;
  for i := 0 to n - 1 do
    Attrs[i] := Attr;
  WriteConsoleOutputAttribute(Out, @Attrs[0], n, Origin(X, Y), Written);
  WriteConsoleOutputCharacterA(Out, @S[1], n, Origin(X, Y), Written);
end;

procedure WriteChar(X, Y, Count: Byte; Ch: Char; Attr: Byte);
var
  Written : DWORD;
  n       : Integer;
begin
  n := Count;
  if Room(X) < n then n := Room(X);
  if n <= 0 then Exit;
  FillConsoleOutputAttribute(Out, Attr, n, Origin(X, Y), Written);
  FillConsoleOutputCharacterA(Out, Ch, n, Origin(X, Y), Written);
end;

procedure Sound(Hz: Word);
begin
  Pending := Hz;
end;

procedure NoSound;
begin
  Pending := 0;
end;

procedure Delay(Ms: Word);
begin
  if (Pending >= 37) and (Ms > 0)
    then Windows.Beep(Pending, Ms)
    else Crt.Delay(Ms);
  Pending := 0;
end;

begin
  Out := GetStdHandle(STD_OUTPUT_HANDLE);
  SetConsoleOutputCP(437);
  SetConsoleCP(437);
  TextMode(CO80);
  Window(1, 1, 80, 25);
  OldWrite := TIOFunc(TextRec(Output).InOutFunc);
  TextRec(Output).InOutFunc := @NewWrite;
  TextRec(Output).FlushFunc := @NewWrite;
end.
