using HFarm.MenuAlimentare.Domain.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFarm.MenuAlimentare.FileSystemStorage
{
    public class CookBookFileSystem : ICookBook
    {
        public const string DEFAULT_STORAGE_FILE_NAME = "cookbook.dat";

        public const char SEPARATOR = ';';

        public string STORAGE_PATH { get; private set; }

        private Dictionary<string, string> _dictionary;

        public CookBookFileSystem()
        {
            init(String.Format(@"{0}\{1}",
                Directory.GetCurrentDirectory(), DEFAULT_STORAGE_FILE_NAME));
        }

        public CookBookFileSystem(string storagePath)
        {
            init(storagePath);
        }

        private void init(string storagePath)
        {
            STORAGE_PATH = storagePath;
            _dictionary = new Dictionary<string, string>();

            if (!File.Exists(storagePath))
            {
                return;
            }

            using (StreamReader reader = new StreamReader(storagePath))
            {
                string currentLine;
                while ((currentLine = reader.ReadLine()) != null)
                {
                    string[] dishes = currentLine
                        .Split(CookBookFileSystem.SEPARATOR)
                        .Select(str => str.Trim()).ToArray<string>();

                    try
                    {
                        _dictionary.Add(dishes[0], dishes[1]);
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                    }
                }
            }
        }

        string ICookBook.Get(string dish)
        {
            string value = null;
            _dictionary.TryGetValue(dish, out value);
            return value;
        }

        void ICookBook.Set(string dish, string dishType)
        {
            _dictionary[dish] = dishType;
            writePair(dish, dishType);
        }

        void ICookBook.Remove(string dish)
        {
            if (dish != null)
                _dictionary.Remove(dish);
        }

        void ICookBook.CleanAndSaveCookBook()
        {
            File.Delete(STORAGE_PATH);
            foreach (KeyValuePair<string, string> pair in _dictionary)
            {
                writePair(pair.Key, pair.Value);
            }
        }

        private void writePair(string dish, string dishType)
        {
            using (StreamWriter writer = new StreamWriter(STORAGE_PATH, true))
            {
                writer.WriteLine("{0}{1}{2}", dish, SEPARATOR, dishType);
            }
        }

        void ICookBook.Clear()
        {
            _dictionary.Clear();
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)_dictionary;
        }

        public string GetFullStoragePath()
        {
            return Path.GetFullPath(STORAGE_PATH);
        }
    }
}
