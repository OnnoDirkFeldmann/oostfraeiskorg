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
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS " +
                "WHERE Deutsch != '-' " +
                "AND Deutsch MATCH @searchstrlower " +
                "UNION SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS " +
                "WHERE Deutsch != '-' " +
                "AND Deutsch MATCH @searchstrupper " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WBFTS " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WBFTS WHERE Deutsch != '-' AND Deutsch MATCH @searchstrlower AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WBFTS WHERE Deutsch != '-' AND Deutsch MATCH @searchstrlower AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Ostfriesisch FROM WBFTS WHERE Deutsch != '-' AND Deutsch MATCH @searchstrupper AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WBFTS WHERE Deutsch != '-' AND Deutsch MATCH @searchstrupper AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "N")
        {
            searchString = "%" + searchString + "%";
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Deutsch != '-' " +
                "AND (Deutsch LIKE @searchstrlower " +
                "OR Deutsch LIKE @searchstrupper) " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WB " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WB WHERE Deutsch != '-' AND (Deutsch LIKE @searchstrlower OR Deutsch LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WB WHERE Deutsch != '-' AND (Deutsch LIKE @searchstrlower OR Deutsch LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "X")
        {
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Deutsch != '-' " +
                "AND Deutsch = @searchstr " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WB " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WB WHERE Deutsch != '-' AND Deutsch = @searchstr AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WB WHERE Deutsch != '-' AND Deutsch = @searchstr AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "B")
        {
            searchString = searchString + "%";
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Deutsch != '-' " +
                "AND (Deutsch LIKE @searchstrlower " +
                "OR Deutsch LIKE @searchstrupper) " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WB " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WB WHERE Deutsch != '-' AND (Deutsch LIKE @searchstrlower OR Deutsch LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WB WHERE Deutsch != '-' AND (Deutsch LIKE @searchstrlower OR Deutsch LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "E")
        {
            searchString = "%" + searchString;
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Deutsch != '-' " +
                "AND (Deutsch LIKE @searchstrlower " +
                "OR Deutsch LIKE @searchstrupper) " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WB " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WB WHERE Deutsch != '-' AND (Deutsch LIKE @searchstrlower OR Deutsch LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WB WHERE Deutsch != '-' AND (Deutsch LIKE @searchstrlower OR Deutsch LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
    }
    public static void FrsDe(string fullTextSearch, ref string searchString, ref SqliteCommand sqlCommand)
    {
        if (fullTextSearch == "J")
        {
            searchString = "\'" + searchString + "\'";
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS " +
                "WHERE Deutsch != '-' " +
                "AND Ostfriesisch MATCH @searchstrlower " +
                "UNION SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS WHERE Deutsch != '-' " +
                "AND Ostfriesisch MATCH @searchstrupper " +
                "UNION SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS WHERE Deutsch != '-' " +
                "AND Nebenformen MATCH @searchstrlower " +
                "UNION SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS WHERE Deutsch != '-' " +
                "AND Nebenformen MATCH @searchstrupper " +
                "UNION SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS WHERE Deutsch != '-' " +
                "AND Plural MATCH @searchstrlower " +
                "UNION SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS WHERE Deutsch != '-' " +
                "AND Plural MATCH @searchstrupper " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WBFTS " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WBFTS WHERE Deutsch != '-' AND Ostfriesisch MATCH @searchstrlower AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WBFTS WHERE Deutsch != '-' AND Ostfriesisch MATCH @searchstrlower AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Ostfriesisch FROM WBFTS WHERE Deutsch != '-' AND Ostfriesisch MATCH @searchstrupper AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WBFTS WHERE Deutsch != '-' AND Ostfriesisch MATCH @searchstrupper AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Nebenformen FROM WBFTS WHERE Deutsch != '-' AND Nebenformen MATCH @searchstrlower AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Nebenformen || '=' || Nummer FROM WBFTS WHERE Deutsch != '-' AND Nebenformen MATCH @searchstrlower AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Nebenformen FROM WBFTS WHERE Deutsch != '-' AND Nebenformen MATCH @searchstrupper AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Nebenformen || '=' || Nummer FROM WBFTS WHERE Deutsch != '-' AND Nebenformen MATCH @searchstrupper AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Plural FROM WBFTS WHERE Deutsch != '-' AND Plural MATCH @searchstrlower AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Plural || '=' || Nummer FROM WBFTS WHERE Deutsch != '-' AND Plural MATCH @searchstrlower AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Plural FROM WBFTS WHERE Deutsch != '-' AND Plural MATCH @searchstrupper AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Plural || '=' || Nummer FROM WBFTS WHERE Deutsch != '-' AND Plural MATCH @searchstrupper AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "N")
        {
            searchString = "%" + searchString + "%";
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Deutsch != '-' AND " +
                "(Ostfriesisch LIKE @searchstrlower " +
                "OR Ostfriesisch LIKE @searchstrupper " +
                "OR Nebenformen LIKE @searchstrlower " +
                "OR Nebenformen LIKE @searchstrupper " +
                "OR Plural LIKE @searchstrlower " +
                "OR Plural LIKE @searchstrupper) " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WB " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WB WHERE Deutsch != '-' AND (Ostfriesisch LIKE @searchstrlower OR Ostfriesisch LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WB WHERE Deutsch != '-' AND (Ostfriesisch LIKE @searchstrlower OR Ostfriesisch LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Nebenformen FROM WB WHERE Deutsch != '-' AND (Nebenformen LIKE @searchstrlower OR Nebenformen LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Nebenformen || '=' || Nummer FROM WB WHERE Deutsch != '-' AND (Nebenformen LIKE @searchstrlower OR Nebenformen LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Plural FROM WB WHERE Deutsch != '-' AND (Plural LIKE @searchstrlower OR Plural LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Plural || '=' || Nummer FROM WB WHERE Deutsch != '-' AND (Plural LIKE @searchstrlower OR Plural LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "X")
        {
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Deutsch != '-' AND " +
                "(Ostfriesisch = @searchstr " +
                "OR Nebenformen = @searchstr " +
                "OR Plural = @searchstr) " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WB " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WB WHERE Deutsch != '-' AND (Ostfriesisch = @searchstr OR Nebenformen = @searchstr OR Plural = @searchstr) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WB WHERE Deutsch != '-' AND (Ostfriesisch = @searchstr OR Nebenformen = @searchstr OR Plural = @searchstr) AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "B")
        {
            searchString = searchString + "%";
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Deutsch != '-' AND " +
                "(Ostfriesisch LIKE @searchstrlower " +
                "OR Ostfriesisch LIKE @searchstrupper " +
                "OR Nebenformen LIKE @searchstrlower " +
                "OR Nebenformen LIKE @searchstrupper " +
                "OR Plural LIKE @searchstrlower " +
                "OR Plural LIKE @searchstrupper) " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WB " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WB WHERE Deutsch != '-' AND (Ostfriesisch LIKE @searchstrlower OR Ostfriesisch LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WB WHERE Deutsch != '-' AND (Ostfriesisch LIKE @searchstrlower OR Ostfriesisch LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Nebenformen FROM WB WHERE Deutsch != '-' AND (Nebenformen LIKE @searchstrlower OR Nebenformen LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Nebenformen || '=' || Nummer FROM WB WHERE Deutsch != '-' AND (Nebenformen LIKE @searchstrlower OR Nebenformen LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Plural FROM WB WHERE Deutsch != '-' AND (Plural LIKE @searchstrlower OR Plural LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Plural || '=' || Nummer FROM WB WHERE Deutsch != '-' AND (Plural LIKE @searchstrlower OR Plural LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "E")
        {
            searchString = "%" + searchString;
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Deutsch != '-' AND " +
                "(Ostfriesisch LIKE @searchstrlower " +
                "OR Ostfriesisch LIKE @searchstrupper " +
                "OR Nebenformen LIKE @searchstrlower " +
                "OR Nebenformen LIKE @searchstrupper " +
                "OR Plural LIKE @searchstrlower " +
                "OR Plural LIKE @searchstrupper) " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WB " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WB WHERE Deutsch != '-' AND (Ostfriesisch LIKE @searchstrlower OR Ostfriesisch LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WB WHERE Deutsch != '-' AND (Ostfriesisch LIKE @searchstrlower OR Ostfriesisch LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Nebenformen FROM WB WHERE Deutsch != '-' AND (Nebenformen LIKE @searchstrlower OR Nebenformen LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Nebenformen || '=' || Nummer FROM WB WHERE Deutsch != '-' AND (Nebenformen LIKE @searchstrlower OR Nebenformen LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Plural FROM WB WHERE Deutsch != '-' AND (Plural LIKE @searchstrlower OR Plural LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Plural || '=' || Nummer FROM WB WHERE Deutsch != '-' AND (Plural LIKE @searchstrlower OR Plural LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
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
                "UNION SELECT *, 1 AS PhraseOrder FROM WBFTS " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WBFTS WHERE Englisch != '-' AND Englisch MATCH @searchstrlower AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WBFTS WHERE Englisch != '-' AND Englisch MATCH @searchstrlower AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Ostfriesisch FROM WBFTS WHERE Englisch != '-' AND Englisch MATCH @searchstrupper AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WBFTS WHERE Englisch != '-' AND Englisch MATCH @searchstrupper AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "N")
        {
            searchString = "%" + searchString + "%";
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Englisch != '-' " +
                "AND (Englisch LIKE @searchstrlower " +
                "OR Englisch LIKE @searchstrupper) " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WB " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WB WHERE Englisch != '-' AND (Englisch LIKE @searchstrlower OR Englisch LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WB WHERE Englisch != '-' AND (Englisch LIKE @searchstrlower OR Englisch LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "X")
        {
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Englisch != '-' AND " +
                "Englisch = @searchstr " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WB " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WB WHERE Englisch != '-' AND Englisch = @searchstr AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WB WHERE Englisch != '-' AND Englisch = @searchstr AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "B")
        {
            searchString = searchString + "%";
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Englisch != '-' " +
                "AND (Englisch LIKE @searchstrlower " +
                "OR Englisch LIKE @searchstrupper) " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WB " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WB WHERE Englisch != '-' AND (Englisch LIKE @searchstrlower OR Englisch LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WB WHERE Englisch != '-' AND (Englisch LIKE @searchstrlower OR Englisch LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "E")
        {
            searchString = "%" + searchString;
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Englisch != '-' " +
                "AND (Englisch LIKE @searchstrlower " +
                "OR Englisch LIKE @searchstrupper) " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WB " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WB WHERE Englisch != '-' AND (Englisch LIKE @searchstrlower OR Englisch LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WB WHERE Englisch != '-' AND (Englisch LIKE @searchstrlower OR Englisch LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
    }
    public static void FrsEn(string fullTextSearch, ref string searchString, ref SqliteCommand sqlCommand)
    {
        if (fullTextSearch == "J")
        {
            searchString = "\'" + searchString + "\'";
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS " +
                "WHERE Englisch != '-' AND " +
                "Ostfriesisch MATCH @searchstrlower " +
                "UNION SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS WHERE Englisch != '-' " +
                "AND Ostfriesisch MATCH @searchstrupper " +
                "UNION SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS WHERE Englisch != '-' " +
                "AND Nebenformen MATCH @searchstrlower " +
                "UNION SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS WHERE Englisch != '-' " +
                "AND Nebenformen MATCH @searchstrupper " +
                "UNION SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS WHERE Englisch != '-' " +
                "AND Plural MATCH @searchstrlower " +
                "UNION SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WBFTS WHERE Englisch != '-' " +
                "AND Plural MATCH @searchstrupper " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WBFTS " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WBFTS WHERE Englisch != '-' AND Ostfriesisch MATCH @searchstrlower AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WBFTS WHERE Englisch != '-' AND Ostfriesisch MATCH @searchstrlower AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Ostfriesisch FROM WBFTS WHERE Englisch != '-' AND Ostfriesisch MATCH @searchstrupper AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WBFTS WHERE Englisch != '-' AND Ostfriesisch MATCH @searchstrupper AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Nebenformen FROM WBFTS WHERE Englisch != '-' AND Nebenformen MATCH @searchstrlower AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Nebenformen || '=' || Nummer FROM WBFTS WHERE Englisch != '-' AND Nebenformen MATCH @searchstrlower AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Nebenformen FROM WBFTS WHERE Englisch != '-' AND Nebenformen MATCH @searchstrupper AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Nebenformen || '=' || Nummer FROM WBFTS WHERE Englisch != '-' AND Nebenformen MATCH @searchstrupper AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Plural FROM WBFTS WHERE Englisch != '-' AND Plural MATCH @searchstrlower AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Plural || '=' || Nummer FROM WBFTS WHERE Englisch != '-' AND Plural MATCH @searchstrlower AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Plural FROM WBFTS WHERE Englisch != '-' AND Plural MATCH @searchstrupper AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Plural || '=' || Nummer FROM WBFTS WHERE Englisch != '-' AND Plural MATCH @searchstrupper AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "N")
        {
            searchString = "%" + searchString + "%";
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Englisch != '-' AND " +
                "(Ostfriesisch LIKE @searchstrlower " +
                "OR Ostfriesisch LIKE @searchstrupper " +
                "OR Nebenformen LIKE @searchstrlower " +
                "OR Nebenformen LIKE @searchstrupper " +
                "OR Plural LIKE @searchstrlower " +
                "OR Plural LIKE @searchstrupper) " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WB " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WB WHERE Englisch != '-' AND (Ostfriesisch LIKE @searchstrlower OR Ostfriesisch LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WB WHERE Englisch != '-' AND (Ostfriesisch LIKE @searchstrlower OR Ostfriesisch LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Nebenformen FROM WB WHERE Englisch != '-' AND (Nebenformen LIKE @searchstrlower OR Nebenformen LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Nebenformen || '=' || Nummer FROM WB WHERE Englisch != '-' AND (Nebenformen LIKE @searchstrlower OR Nebenformen LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Plural FROM WB WHERE Englisch != '-' AND (Plural LIKE @searchstrlower OR Plural LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Plural || '=' || Nummer FROM WB WHERE Englisch != '-' AND (Plural LIKE @searchstrlower OR Plural LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "X")
        {
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Englisch != '-' AND " +
                "(Ostfriesisch = @searchstr " +
                "OR Nebenformen = @searchstr " +
                "OR Plural = @searchstr) " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WB " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WB WHERE Englisch != '-' AND (Ostfriesisch = @searchstr OR Nebenformen = @searchstr OR Plural = @searchstr) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WB WHERE Englisch != '-' AND (Ostfriesisch = @searchstr OR Nebenformen = @searchstr OR Plural = @searchstr) AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "B")
        {
            searchString = searchString + "%";
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Englisch != '-' AND " +
                "(Ostfriesisch LIKE @searchstrlower " +
                "OR Ostfriesisch LIKE @searchstrupper " +
                "OR Nebenformen LIKE @searchstrlower " +
                "OR Nebenformen LIKE @searchstrupper " +
                "OR Plural LIKE @searchstrlower " +
                "OR Plural LIKE @searchstrupper) " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WB " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WB WHERE Englisch != '-' AND (Ostfriesisch LIKE @searchstrlower OR Ostfriesisch LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WB WHERE Englisch != '-' AND (Ostfriesisch LIKE @searchstrlower OR Ostfriesisch LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Nebenformen FROM WB WHERE Englisch != '-' AND (Nebenformen LIKE @searchstrlower OR Nebenformen LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Nebenformen || '=' || Nummer FROM WB WHERE Englisch != '-' AND (Nebenformen LIKE @searchstrlower OR Nebenformen LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Plural FROM WB WHERE Englisch != '-' AND (Plural LIKE @searchstrlower OR Plural LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Plural || '=' || Nummer FROM WB WHERE Englisch != '-' AND (Plural LIKE @searchstrlower OR Plural LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
        if (fullTextSearch == "E")
        {
            searchString = "%" + searchString;
            sqlCommand.CommandText = "SELECT *, CASE WHEN Wortart = 'Phrase' THEN 1 ELSE 0 END AS PhraseOrder FROM WB " +
                "WHERE Englisch != '-' AND " +
                "(Ostfriesisch LIKE @searchstrlower " +
                "OR Ostfriesisch LIKE @searchstrupper " +
                "OR Nebenformen LIKE @searchstrlower " +
                "OR Nebenformen LIKE @searchstrupper " +
                "OR Plural LIKE @searchstrlower " +
                "OR Plural LIKE @searchstrupper) " +
                "UNION SELECT *, 1 AS PhraseOrder FROM WB " +
                "WHERE Wortart = 'Phrase' AND (Zuordnung IN (SELECT Ostfriesisch FROM WB WHERE Englisch != '-' AND (Ostfriesisch LIKE @searchstrlower OR Ostfriesisch LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Ostfriesisch || '=' || Nummer FROM WB WHERE Englisch != '-' AND (Ostfriesisch LIKE @searchstrlower OR Ostfriesisch LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Nebenformen FROM WB WHERE Englisch != '-' AND (Nebenformen LIKE @searchstrlower OR Nebenformen LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Nebenformen || '=' || Nummer FROM WB WHERE Englisch != '-' AND (Nebenformen LIKE @searchstrlower OR Nebenformen LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-') " +
                "OR Zuordnung IN (SELECT Plural FROM WB WHERE Englisch != '-' AND (Plural LIKE @searchstrlower OR Plural LIKE @searchstrupper) AND Wortart != 'Phrase') " +
                "OR Zuordnung IN (SELECT Plural || '=' || Nummer FROM WB WHERE Englisch != '-' AND (Plural LIKE @searchstrlower OR Plural LIKE @searchstrupper) AND Wortart != 'Phrase' AND Nummer != '-')) " +
                "ORDER BY PhraseOrder ASC, Ostfriesisch ASC";
        }
    }
}