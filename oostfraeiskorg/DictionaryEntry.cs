using System.Collections.Generic;

namespace oostfraeiskorg;

public class DictionaryEntry
{
    public string Frisian { get; set; }
    public string FrisianWithArticle { get; set; }
    public string SecondaryForm { get; set; }
    public string StandardForm { get; set; }
    public string Translation { get; set; }
    public bool SoundFile { get; set; }
    public string MP3 { get; set; }
    public long ID { get; set; }
    public string Number { get; set; }
    public bool IsPhrase { get; set; }
    public string PhraseParent { get; set; }
    public List<DictionaryEntry> Phrases { get; set; }
    public bool HasPhrases => Phrases != null && Phrases.Count > 0;

    public DictionaryEntry()
    {
        Phrases = new List<DictionaryEntry>();
    }

    public DictionaryEntry(string frisian, string frisianWithArticle, string secondaryform, string standardform, string translation, long id, string number = "-", bool isPhrase = false, string phraseParent = "")
    {
        Frisian = frisian;
        FrisianWithArticle = frisianWithArticle;
        SecondaryForm = secondaryform;
        StandardForm = standardform;
        Translation = translation;
        ID = id;
        Number = number;
        SoundFile = false;
        MP3 = "";
        IsPhrase = isPhrase;
        PhraseParent = phraseParent;
        Phrases = new List<DictionaryEntry>();
    }
}
