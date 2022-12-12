using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RoadSectionHandler.Utilities
{

    public enum FILTER_TYPE
    {
        EQUALS,
        IN,
        LIKE,
        LESS,
        GREATER,
        LESS_EQ,
        GREATER_EQ
    }

    public enum PREV_JOIN
    {
        AND,
        OR
    }

    public class PsqlFilter
    {

        public string fieldName { get; set; }
        public FILTER_TYPE type { get; set; }
        public object value { get; set; }

        public PREV_JOIN join { get; set; } = PREV_JOIN.AND; 

        public PsqlFilter(string fieldName, FILTER_TYPE type, object value)
        {
            this.fieldName = fieldName;
            this.type = type;
            this.value = value;
        }

        private string handleJoinEnumWithValue(FILTER_TYPE filt)
        {
            switch (filt)
            {
                case FILTER_TYPE.EQUALS:
                    return "=";
                case FILTER_TYPE.LESS:
                    return "<";
                case FILTER_TYPE.GREATER:
                    return ">";
                case FILTER_TYPE.LESS_EQ:
                    return "<=";
                case FILTER_TYPE.GREATER_EQ:
                    return ">=";
                case FILTER_TYPE.IN:
                    return "IN";
                case FILTER_TYPE.LIKE:
                    return "LIKE";
                default:
                    return "";
            }
        }

        private string handleValueByType(object value)
        {
            string returnString = "(";
            if (value.GetType().IsArray)
            {
                foreach (object o in (Array)value)
                {

                    returnString += "'" + o.ToString() + "',";

                }
                returnString = returnString.Remove(returnString.Length - 1);
                returnString += ")";
            }
            else if (value is IList && value.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
            {

                foreach (var listitem in value as IEnumerable)
                {
                    returnString += "'" + listitem.ToString() + "',";
                }
                returnString = returnString.Remove(returnString.Length - 1);
                returnString += ")";
            }
            else
            {
                returnString = "'" + value.ToString() + "'";
            }

            return returnString;
        }

        public string ResolveFilter(bool first)
        {

            string filt = "";
            
            if (!first)
            {
                filt += join.ToString() + " ";
            }
            else
            {
                filt += " ";
            }

            filt += fieldName + " " + handleJoinEnumWithValue(this.type) + " " + handleValueByType(this.value);
            return filt;
        }

    }
}
