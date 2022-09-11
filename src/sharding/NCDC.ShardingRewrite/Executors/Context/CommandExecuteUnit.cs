using NCDC.Basic.Connection.Abstractions;
using NCDC.Enums;

namespace NCDC.ShardingRewrite.Executors.Context;


public sealed class CommandExecuteUnit
{
    public ExecutionUnit ExecutionUnit { get; }
    
    public IServerDbCommand ServerDbCommand{ get; }
    
    public ConnectionModeEnum ConnectionMode{ get; }

    public CommandExecuteUnit(ExecutionUnit executionUnit, IServerDbCommand serverDbCommand, ConnectionModeEnum connectionMode)
    {
        ExecutionUnit = executionUnit;
        ServerDbCommand = serverDbCommand;
        ConnectionMode = connectionMode;
    }
}