
using System.Collections.Generic;

namespace CountingApp
{
    public class Input
    {
        public List<Records> values {get; set;}

    }


    public class Output
    {
        public List<Records> values {get; set;}
    }


    public class Records{
        public string recordId {get; set;}
        public wordList data {get; set;}
    }

    public class wordList {
        public string mergedText {get;set;}
        public List<string> words {get;set;}
    }

}