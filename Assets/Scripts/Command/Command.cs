using System;
using System.Collections.Generic;

public abstract class Command
{
    ICommandReciever _reciever = null;

    public Command(ICommandReciever reciever)
    {
        _reciever = reciever;
    }

    public virtual void Execute()
    {
        _reciever.DoAction();
    }
}
