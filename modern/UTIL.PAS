unit Util;

interface

  function Upper( st: string ): string;

implementation

uses 
  CRT, DOS;

var
  OldExitProc: pointer;

  function Upper( st: string ): string;
    var
      i : byte;
      s : string;
    begin
      s := '';
      for i := 1 to length( st ) do
	s := s + upcase( st[i] );
      Upper := s;
    end;

  procedure MyExit; far;
  begin
    ExitProc := OldExitProc;

    textbackground( 0 );
    textcolor( 7 );
    if ErrorAddr <> nil
      then
	begin
	  write('Ha ocurrido el error interno #',ExitCode,' en ');
	  write(HexStr(PtrUInt(ErrorAddr), 8));
	  writeln('. Por favor, reporte a Merchise.'#10);
	  ErrorAddr := nil
	end
      else
	case ExitCode of
	 1: writeln( 'Ctrl-Break fue presionado. Utilice "Terminar" para una salida m s elegante.' );
	 2: writeln( '[[]] Lo siento, pero no hay memoria suficiente para inicializar La Fortaleza.' );
	end;
  end;

begin
  OldExitProc := ExitProc;
{$IFNDEF NOEXIT}
  ExitProc := @MyExit;
{$ENDIF}
end.
