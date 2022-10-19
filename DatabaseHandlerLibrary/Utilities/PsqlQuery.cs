using RoadSectionHandler.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadSectionHandler
{
    public class PsqlQuery
    {

        public List<PsqlFilter> Filters { get; set; } = new List<PsqlFilter>();

        public List<string> Fields { get; set; } = new List<string>();

        // limit
        public PsqlLimit Limit { get; set; }

        public List<PsqlJoin> Joins { get; set; } = new List<PsqlJoin>();

        public string TableName { get; set; }

        public PsqlQuery(string table, List<string> fields)
        {

            this.TableName = table;
            this.Fields = fields;
        }

        public void AddFilter(PsqlFilter filter, PREV_JOIN joinPrev)
        {
            if(joinPrev != null)
            {
                filter.join = joinPrev;
            }

            if(filter != null)
            {
                Filters.Add(filter);
            }

        }
        public void AddFilter(PsqlFilter filter)
        {
            this.AddFilter(filter, PREV_JOIN.AND);

        }

        public void AddJoin(PsqlJoin join)
        {
            if (join != null)
            {
                Joins.Add(join);
            }

        }

        public string ResolveQuery()
        {
            string query = "SELECT " + String.Join(",", Fields) + " FROM " + TableName;

            if(Joins.Count > 0)
            {

                foreach(PsqlJoin join in Joins)
                {
                    query += " " + join.ResolveJoin();
                }

            }

            if (Filters.Count > 0)
            {
                bool first = true;
                foreach (PsqlFilter filter in Filters)
                {
                    query += " " + filter.ResolveFilter(first);
                    first = false;
                }

            }

            return query + " " + Limit.ResolveLimit();

        }

    }
}
