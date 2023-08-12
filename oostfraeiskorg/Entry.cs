namespace oostfraeiskorg
{
    public class Entry
    {
        public string Frisian { get; set; }
        public string SecondaryForm { get; set; }
        public string StandardForm { get; set; }
        public string Translation { get; set; }
        public bool SoundFile { get; set; }
        public string MP3 { get; set; }
        public long ID { get; set; }

        public Entry()
        {
            // NOTE: This default constructor is required. 
            // Remember that the viewmodel is JSON-serialized
            // which requires all objects to have a public 
            // parameterless constructor
        }

        public Entry(string frisian, string secondaryform, string standardform, string translation, long id)
        {
            Frisian = frisian;
            SecondaryForm = secondaryform;
            StandardForm = standardform;
            Translation = translation;
            ID = id;
            SoundFile = false;
            MP3 = "";
        }
    }
}
