﻿using System.Data;
using System.Linq;
using KellermanSoftware.CompareNetObjects;

namespace NDbUnit.Core
{
    public static class DataSetComparer
    {

        private static CompareLogic _comparer = new CompareLogic();

        public static bool HasSameDataAs(this DataSet left, DataSet right)
        {
            //if schemas don't match, no point in proceeding to test any data/content so just bail out early...
            if (!left.HasSameSchemaAs(right))
                return false;

            return false;
        }

        public static bool HasSameSchemaAs(this DataSet left, DataSet right)
        {
            //if the count of tables fails to match, no point in proceeding
            if (left.Tables.Count != right.Tables.Count)
                return false;

            //consider tables
            foreach (var table in left.Tables.Cast<DataTable>())
            {
                if (!right.Tables.Contains(table.TableName))
                    return false;

                if (!HaveTheSameSchema(table, right.Tables[table.TableName]))
                    return false;
            }

            //consider relatioships
            foreach (var relationship in left.Relations.Cast<DataRelation>())
            {
                if (!HaveTheSameSchema(relationship, right.Relations[relationship.RelationName]))
                    return false;
            }

            return true;
        }

        private static bool HaveTheSameSchema(DataRelation left, DataRelation right)
        {
            var config = new ComparisonConfig { IgnoreCollectionOrder = true, CompareChildren = false };
            _comparer.Config = config;

            var result = _comparer.Compare(left, right);

            return result.AreEqual;
        }

        private static bool HaveTheSameSchema(DataTable left, DataTable right)
        {
            var config = new ComparisonConfig { IgnoreCollectionOrder = true, CompareChildren = false };
            config.MembersToIgnore.Add("Columns");
            config.MembersToIgnore.Add("Rows");

            _comparer.Config = config;

            var result = _comparer.Compare(left, right);

            if (!result.AreEqual)
                return false;

            //if the count of columns fails to match, no point in proceeding
            if (left.Columns.Count != right.Columns.Count)
                return false;

            foreach (var column in left.Columns.Cast<DataColumn>())
            {
                if (!right.Columns.Contains(column.ColumnName))
                    return false;

                if (!HaveTheSameSchema(column, right.Columns[column.ColumnName]))
                    return false;
            }

            return true;
        }

        private static bool HaveTheSamData(DataRow left, DataRow right)
        {
            return false;
        }

        private static bool HaveTheSameSchema(DataColumn left, DataColumn right)
        {
            var config = new ComparisonConfig { IgnoreCollectionOrder = true, CompareChildren = false };
            _comparer.Config = config;

            var result = _comparer.Compare(left, right);

            return result.AreEqual;
        }
    }
}