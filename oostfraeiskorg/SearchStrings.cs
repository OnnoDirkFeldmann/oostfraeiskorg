using DotVVM.Framework.Controls;
using Microsoft.Data.Sqlite;

namespace oostfraeiskorg;

public class SearchStrings
{
    public static void DeFrs(string fullTextSearch, ref string searchString, ref SqliteCommand sqlCommand)
    {
        if (fullTextSearch == "J")
        {
            searchString = "\'" + searchString + "\'";
            /*sqlCommand.CommandText = "SELECT * FROM WBFTS " +
                "WHERE Deutsch != '-' " +
                "AND Deutsch MATCH @searchstrlower " +
                "UNION SELECT * FROM WBFTS " +
                "WHERE Deutsch != '-' " +
                "AND Deutsch MATCH @searchstrupper " +
                "ORDER BY Ostfriesisch ASC";*/

            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS " +
                "WHERE Deutsch != '-' " +
                "AND Deutsch MATCH @searchstrlower " +
                "UNION SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS " +
                "WHERE Deutsch != '-' " +
                "AND Deutsch MATCH @searchstrupper " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";

        }
        if (fullTextSearch == "N")
        {
            searchString = "%" + searchString + "%";
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Deutsch != '-' " +
                "AND (Deutsch LIKE @searchstrlower " +
                "OR Deutsch LIKE @searchstrupper) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "X")
        {
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Deutsch != '-' " +
                "AND Deutsch = @searchstr " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "B")
        {
            searchString = searchString + "%";
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Deutsch != '-' " +
                "AND (Deutsch LIKE @searchstrlower " +
                "OR Deutsch LIKE @searchstrupper) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "E")
        {
            searchString = "%" + searchString;
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Deutsch != '-' " +
                "AND (Deutsch LIKE @searchstrlower " +
                "OR Deutsch LIKE @searchstrupper) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
    }
    public static void FrsDe(string fullTextSearch, ref string searchString, ref SqliteCommand sqlCommand)
    {
        if (fullTextSearch == "J")
        {
            searchString = "\'" + searchString + "\'";
            sqlCommand.CommandText = "SELECT * FROM WBFTS " +
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
        if (fullTextSearch == "N")
        {
            searchString = "%" + searchString + "%";
            sqlCommand.CommandText = "SELECT * FROM WB " +
                "WHERE Deutsch != '-' AND " +
                "(Ostfriesisch LIKE @searchstrlower " +
                "OR Ostfriesisch LIKE @searchstrupper " +
                "OR Nebenformen LIKE @searchstrlower " +
                "OR Nebenformen LIKE @searchstrupper " +
                "OR Plural LIKE @searchstrlower " +
                "OR Plural LIKE @searchstrupper) " +
                "ORDER BY Ostfriesisch ASC";
        }
        if (fullTextSearch == "X")
        {
            sqlCommand.CommandText = "SELECT * FROM WB " +
                "WHERE Deutsch != '-' AND " +
                "(Ostfriesisch = @searchstr " +
                "OR Nebenformen = @searchstr " +
                "OR Plural = @searchstr) " +
                "ORDER BY Ostfriesisch ASC";
        }
        if (fullTextSearch == "B")
        {
            searchString = searchString + "%";
            sqlCommand.CommandText = "SELECT * FROM WB " +
                "WHERE Deutsch != '-' AND " +
                "(Ostfriesisch LIKE @searchstrlower " +
                "OR Ostfriesisch LIKE @searchstrupper " +
                "OR Nebenformen LIKE @searchstrlower " +
                "OR Nebenformen LIKE @searchstrupper " +
                "OR Plural LIKE @searchstrlower " +
                "OR Plural LIKE @searchstrupper) " +
                "ORDER BY Ostfriesisch ASC";
        }
        if (fullTextSearch == "E")
        {
            searchString = "%" + searchString;
            sqlCommand.CommandText = "SELECT * FROM WB " +
                "WHERE Deutsch != '-' AND " +
                "(Ostfriesisch LIKE @searchstrlower " +
                "OR Ostfriesisch LIKE @searchstrupper " +
                "OR Nebenformen LIKE @searchstrlower " +
                "OR Nebenformen LIKE @searchstrupper " +
                "OR Plural LIKE @searchstrlower " +
                "OR Plural LIKE @searchstrupper) " +
                "ORDER BY Ostfriesisch ASC";
        }
    }

    public static void EnFrs(string fullTextSearch, ref string searchString, ref SqliteCommand sqlCommand)
    {
        if (fullTextSearch == "J")
        {
            searchString = "\'" + searchString + "\'";
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS " +
                "WHERE Englisch != '-' " +
                "AND Englisch MATCH @searchstrlower " +
                "UNION SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS " +
                "WHERE Englisch != '-' " +
                "AND Englisch MATCH @searchstrupper " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "N")
        {
            searchString = "%" + searchString + "%";
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Englisch != '-' " +
                "AND (Englisch LIKE @searchstrlower " +
                "OR Englisch LIKE @searchstrupper) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "X")
        {
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Englisch != '-' AND " +
                "Englisch = @searchstr " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "B")
        {
            searchString = searchString + "%";
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Englisch != '-' " +
                "AND (Englisch LIKE @searchstrlower " +
                "OR Englisch LIKE @searchstrupper) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "E")
        {
            searchString = "%" + searchString;
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Englisch != '-' " +
                "AND (Englisch LIKE @searchstrlower " +
                "OR Englisch LIKE @searchstrupper) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
    }
    public static void FrsEn(string fullTextSearch, ref string searchString, ref SqliteCommand sqlCommand)
    {
        if (fullTextSearch == "J")
        {
            searchString = "\'" + searchString + "\'";
            sqlCommand.CommandText = "SELECT * FROM WBFTS " +
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
        if (fullTextSearch == "N")
        {
            searchString = "%" + searchString + "%";
            sqlCommand.CommandText = "SELECT * FROM WB " +
                "WHERE Englisch != '-' AND " +
                "(Ostfriesisch LIKE @searchstrlower " +
                "OR Ostfriesisch LIKE @searchstrupper " +
                "OR Nebenformen LIKE @searchstrlower " +
                "OR Nebenformen LIKE @searchstrupper " +
                "OR Plural LIKE @searchstrlower " +
                "OR Plural LIKE @searchstrupper) " +
                "ORDER BY Ostfriesisch ASC";
        }
        if (fullTextSearch == "X")
        {
            sqlCommand.CommandText = "SELECT * FROM WB " +
                "WHERE Englisch != '-' AND " +
                "(Ostfriesisch = @searchstr " +
                "OR Nebenformen = @searchstr " +
                "OR Plural = @searchstr) " +
                "ORDER BY Ostfriesisch ASC";
        }
        if (fullTextSearch == "B")
        {
            searchString = searchString + "%";
            sqlCommand.CommandText = "SELECT * FROM WB " +
                "WHERE Englisch != '-' AND " +
                "(Ostfriesisch LIKE @searchstrlower " +
                "OR Ostfriesisch LIKE @searchstrupper " +
                "OR Nebenformen LIKE @searchstrlower " +
                "OR Nebenformen LIKE @searchstrupper " +
                "OR Plural LIKE @searchstrlower " +
                "OR Plural LIKE @searchstrupper) " +
                "ORDER BY Ostfriesisch ASC";
        }
        if (fullTextSearch == "E")
        {
            searchString = "%" + searchString;
            sqlCommand.CommandText = "SELECT * FROM WB " +
                "WHERE Englisch != '-' AND " +
                "(Ostfriesisch LIKE @searchstrlower " +
                "OR Ostfriesisch LIKE @searchstrupper " +
                "OR Nebenformen LIKE @searchstrlower " +
                "OR Nebenformen LIKE @searchstrupper " +
                "OR Plural LIKE @searchstrlower " +
                "OR Plural LIKE @searchstrupper) " +
                "ORDER BY Ostfriesisch ASC";
        }
    }
}