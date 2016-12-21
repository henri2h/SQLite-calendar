﻿using SQL_lite_database_search_wpf.Core.DatabaseItems.objects;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_lite_database_search_wpf.Core.DatabaseManager.ObjectsManager
{
    public class CalendarObjectManager
    {
        public const string elementID = "elementID";

        public void createCalendarTable(string tableName)
        {
            calendarObject cObj = new calendarObject();

            StringBuilder sbElements = new StringBuilder();
            {
                bool justCreated = true;

                foreach (sqliteBase sElement in cObj.values)
                {
                    if (justCreated) { justCreated = false; }
                    else { sbElements.Append(", "); }
                    sbElements.Append(sElement.valueName + " " + sElement.dataType);

                }
            }

            string sql_command = "CREATE TABLE " + tableName + " ( " + sbElements.ToString() + ")";
            SQLiteCommandsExecuter.executeNonQuery(sql_command);
        }


        /// <summary>
        /// Add the calendar object to the table and create it associated projecttablename if needed
        /// </summary>
        /// <param name="cObj"></param>
        public void addCalendarObject(calendarObject cObj, bool createAssociatedTable = false)
        {
            try
            {
                if (cObj.tableName == null) { throw new ArgumentNullException(); }

                // associate project table name :
                // ============

                if (cObj.isRepository.value)
                {
                    if ((cObj.isRepository.value && createAssociatedTable) || (cObj.projectTableName.value == null || cObj.projectTableName.value == ""))
                    {
                        cObj.projectTableName.value = "Element_" + Guid.NewGuid().ToString("n") + "_Tasks";
                    }
                    // else just use the current table name
                    createCalendarTable(cObj.projectTableName.value);

                }

                Request.RequestBuilder rb = new Request.RequestBuilder(cObj.tableName);

                // name and escription
                rb.addElement(cObj.name.valueName, cObj.name.value);
                rb.addElement(cObj.description.valueName, cObj.description.value);


                // priority and completion
                rb.addElement(cObj.priorite.valueName, cObj.priorite.value);
                rb.addElement(cObj.completion.valueName, cObj.completion.value);


                // team
                rb.addElement(cObj.equipe.valueName, cObj.equipe.value);

                // time
                rb.addElement(cObj.startTime.valueName, cObj.startTime.value);
                rb.addElement(cObj.endTime.valueName, cObj.endTime.value);
                rb.addElement(cObj.isDateUsed.valueName, cObj.isDateUsed.value);


                // is project ?
                rb.addElement(cObj.isRepository.valueName, cObj.isRepository.value);
                rb.addElement(cObj.projectTableName.valueName, cObj.projectTableName.value);


                SQLiteCommand cmd = Request.CommandBuilder.getCommand(rb);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ex.Source = "SQL_lite_database_search_wpf.Core.DatabaseManager.ObjectsManager.CalendarObjectManager.addCalendarObject";
                ErrorHandeler.ErrorMessage.logError(ex);

            }

        }
        public void deleteCalendarObject(int id, string tableName)
        {
            sqliteInt sIntID = new sqliteInt("elementID");
            sIntID.value = id;

            AppCore.dCore.delElement(tableName, sIntID);
        }
        public void deleteCalendarObject(calendarObject cObj, bool deleteAssociatedTable = true)
        {
            if (deleteAssociatedTable)
            {
                if (cObj.isRepository.value) { deleteCalendarTable(cObj.projectTableName.value); }
            }

            deleteCalendarObject(cObj.elementID.value, cObj.tableName);
        }

        public void deleteCalendarTable(string tableName, bool deleteAssociatedTable = true)
        {
            if (deleteAssociatedTable)
            {
                List<calendarObject> cObjs = listCalendarObjects(tableName);

                foreach (calendarObject c in cObjs)
                {
                    if (c.isRepository.value) { deleteCalendarTable(c.projectTableName.value); }
                }
            }
            Core.AppCore.dCore.delTable(tableName);
        }


        public calendarObject getCalendarObject(string objectID, string tableName)
        {
            string sql = "select * from " + tableName + " where (" + elementID + " = " + objectID + ")  order by _rowid_ ASC";
            SQLiteDataReader reader = SQLiteCommandsExecuter.executeDataReader(sql);
            calendarObject cObjects = ObjectManager.readerToCobj(reader, tableName);
            return cObjects;

        }


        public List<calendarObject> listCalendarObjects(string tableName)
        {
            string sql = "select * from " + tableName + " order by _rowid_ ASC";
            SQLiteDataReader reader = SQLiteCommandsExecuter.executeDataReader(sql);
            List<calendarObject> cObjects = ObjectManager.readerToCobjs(reader, tableName);
            return cObjects;
        }


        public void updateCalendarObject(sqliteBase sb, int objectID, string tableName)
        {

            string request = "update " + tableName + " set '" + sb.valueName + "'='" + sb.value + "' where '_rowid_'='" + objectID + "'";
            SQLiteCommandsExecuter.executeNonQuery(request);

        }
    }
}
