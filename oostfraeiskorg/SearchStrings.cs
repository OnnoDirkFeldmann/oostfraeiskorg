using Microsoft.Data.Sqlite;

namespace WFDOT
{
    public class SearchStrings
    {
        public static void DeFrs(string volltextsuche, ref string searchstr, ref SqliteCommand sqlcmd)
        {
            if (volltextsuche == "J")
            {
                searchstr = "\'" + searchstr + "\'";
                sqlcmd.CommandText = "SELECT * FROM WBFTS " +
                    "WHERE Deutsch != '-' " +
                    "AND Deutsch MATCH @searchstrlower " +
                    "UNION SELECT * FROM WBFTS " +
                    "WHERE Deutsch != '-' " +
                    "AND Deutsch MATCH @searchstrupper " +
                    "ORDER BY Ostfriesisch ASC";
            }
            if (volltextsuche == "N")
            {
                searchstr = "%" + searchstr + "%";
                sqlcmd.CommandText = "SELECT * FROM WB " +
                    "WHERE Deutsch != '-' " +
                    "AND (Deutsch LIKE @searchstrlower " +
                    "OR Deutsch LIKE @searchstrupper) " +
                    "ORDER BY Ostfriesisch ASC";
            }
            if (volltextsuche == "X")
            {
                sqlcmd.CommandText = "SELECT * FROM WB " +
                    "WHERE Deutsch != '-' " +
                    "AND Deutsch = @searchstr " +
                    "ORDER BY Ostfriesisch ASC";
            }
        }
        public static void FrsDe(string volltextsuche, ref string searchstr, ref SqliteCommand sqlcmd)
        {
            if (volltextsuche == "J")
            {
                searchstr = "\'" + searchstr + "\'";
                sqlcmd.CommandText = "SELECT * FROM WBFTS " +
                    "WHERE Deutsch != '-' " +
                    "AND Ostfriesisch MATCH @searchstrlower " +
                    "UNION SELECT * FROM WBFTS WHERE Deutsch != '-' " +
                    "AND Ostfriesisch MATCH @searchstrupper " +
                    "UNION SELECT * FROM WBFTS WHERE Deutsch != '-' " +
                    "AND Nebenformen MATCH @searchstrlower " +
                    "UNION SELECT * FROM WBFTS WHERE Deutsch != '-' " +
                    "AND Nebenformen MATCH @searchstrupper " +
                    "UNION SELECT * FROM WBFTS WHERE Deutsch != '-' " +
                    "AND Plural MATCH @searchstrlower " +
                    "UNION SELECT * FROM WBFTS WHERE Deutsch != '-' " +
                    "AND Plural MATCH @searchstrupper " +
                    "ORDER BY Ostfriesisch ASC";
            }
            if (volltextsuche == "N")
            {
                searchstr = "%" + searchstr + "%";
                sqlcmd.CommandText = "SELECT * FROM WB " +
                    "WHERE Deutsch != '-' AND " +
                    "(Ostfriesisch LIKE @searchstrlower " +
                    "OR Ostfriesisch LIKE @searchstrupper " +
                    "OR Nebenformen LIKE @searchstrlower " +
                    "OR Nebenformen LIKE @searchstrupper " +
                    "OR Plural LIKE @searchstrlower " +
                    "OR Plural LIKE @searchstrupper) " +
                    "ORDER BY Ostfriesisch ASC";
            }
            if (volltextsuche == "X")
            {
                sqlcmd.CommandText = "SELECT * FROM WB " +
                    "WHERE Deutsch != '-' AND " +
                    "(Ostfriesisch = @searchstr " +
                    "OR Nebenformen = @searchstr " +
                    "OR Plural = @searchstr) " +
                    "ORDER BY Ostfriesisch ASC";
            }
        }

        public static void EnFrs(string volltextsuche, ref string searchstr, ref SqliteCommand sqlcmd)
        {
            if (volltextsuche == "J")
            {
                searchstr = "\'" + searchstr + "\'";
                sqlcmd.CommandText = "SELECT * FROM WBFTS " +
                    "WHERE Englisch != '-' " +
                    "AND Englisch MATCH @searchstrlower " +
                    "UNION SELECT * FROM WBFTS " +
                    "WHERE Englisch != '-' " +
                    "AND Englisch MATCH @searchstrupper " +
                    "ORDER BY Ostfriesisch ASC";
            }
            if (volltextsuche == "N")
            {
                searchstr = "%" + searchstr + "%";
                sqlcmd.CommandText = "SELECT * FROM WB " +
                    "WHERE Englisch != '-' " +
                    "AND (Englisch LIKE @searchstrlower " +
                    "OR Englisch LIKE @searchstrupper) " +
                    "ORDER BY Ostfriesisch ASC";
            }
            if (volltextsuche == "X")
            {
                sqlcmd.CommandText = "SELECT * FROM WB " +
                    "WHERE Englisch != '-' AND " +
                    "Englisch = @searchstr " +
                    "ORDER BY Ostfriesisch ASC";
            }
        }
        public static void FrsEn(string volltextsuche, ref string searchstr, ref SqliteCommand sqlcmd)
        {
            if (volltextsuche == "J")
            {
                searchstr = "\'" + searchstr + "\'";
                sqlcmd.CommandText = "SELECT * FROM WBFTS " +
                    "WHERE Englisch != '-' AND " +
                    "Ostfriesisch MATCH @searchstrlower " +
                    "UNION SELECT * FROM WBFTS WHERE Englisch != '-' " +
                    "AND Ostfriesisch MATCH @searchstrupper " +
                    "UNION SELECT * FROM WBFTS WHERE Englisch != '-' " +
                    "AND Nebenformen MATCH @searchstrlower " +
                    "UNION SELECT * FROM WBFTS WHERE Englisch != '-' " +
                    "AND Nebenformen MATCH @searchstrupper " +
                    "UNION SELECT * FROM WBFTS WHERE Englisch != '-' " +
                    "AND Plural MATCH @searchstrlower " +
                    "UNION SELECT * FROM WBFTS WHERE Englisch != '-' " +
                    "AND Plural MATCH @searchstrupper " +
                    "ORDER BY Ostfriesisch ASC";
            }
            if (volltextsuche == "N")
            {
                searchstr = "%" + searchstr + "%";
                sqlcmd.CommandText = "SELECT * FROM WB " +
                    "WHERE Englisch != '-' AND " +
                    "(Ostfriesisch LIKE @searchstrlower " +
                    "OR Ostfriesisch LIKE @searchstrupper " +
                    "OR Nebenformen LIKE @searchstrlower " +
                    "OR Nebenformen LIKE @searchstrupper " +
                    "OR Plural LIKE @searchstrlower " +
                    "OR Plural LIKE @searchstrupper) " +
                    "ORDER BY Ostfriesisch ASC";
            }
            if (volltextsuche == "X")
            {
                sqlcmd.CommandText = "SELECT * FROM WB " +
                    "WHERE Englisch != '-' AND " +
                    "(Ostfriesisch = @searchstr " +
                    "OR Nebenformen = @searchstr " +
                    "OR Plural = @searchstr) " +
                    "ORDER BY Ostfriesisch ASC";
            }
        }
    }
}