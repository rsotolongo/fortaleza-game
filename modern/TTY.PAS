unit TTY;

interface

  uses
    Objects, HistTTY;

  const
    UseEGA: boolean = false;

  function GetTTY( R : TRect ) : PHistTTY;
  function IsEGA               : boolean;

implementation

  uses
    CgaTTY;

  function IsEGA : boolean;
    begin
      IsEGA := false
    end;

  function GetTTY( R: TRect ): PHistTTY;
    begin
      GetTTY := new( PCgaTTY, Init( R ))
    end;

end.
