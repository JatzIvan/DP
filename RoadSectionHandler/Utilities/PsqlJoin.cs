using System;
using System.Collections.Generic;
using System.Text;

namespace RoadSectionHandler.Utilities
{
    public class PsqlJoin
    {

        public string TableToJoin { get; set; }
        public string JoinOnCondition { get; set; }

        public JOIN_TYPE JoinType { get; set; } = JOIN_TYPE.INNER;

        public PsqlJoin(string TableToJoin, string JoinOnCondition) : this(TableToJoin, JoinOnCondition, JOIN_TYPE.INNER)
        {

        }

        public PsqlJoin(string tableToJoin, string joinOnCondition, JOIN_TYPE joinType)
        {
            this.TableToJoin = tableToJoin;
            this.JoinOnCondition = joinOnCondition;
            this.JoinType = joinType;
        }

        public string ResolveJoin()
        {
            return JoinType.ToString() + " JOIN " + TableToJoin + " ON " + JoinOnCondition;
        }

    }

    public enum JOIN_TYPE
    {
        INNER,
        LEFT,
        RIGHT
    }
}
