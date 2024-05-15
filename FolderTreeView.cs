using System.Collections.ObjectModel;

namespace Hcode
{
    public class FolderItem
    {
        public string Name
        {
            get; set;
        }
        public ObservableCollection<object> SubItems
        {
            get; set;
        }

        public FolderItem(string name)
        {
            Name = name;
            SubItems = new ObservableCollection<object>();
        }
    }

    public class FileItem
    {
        public string Name
        {
            get; set;
        }

        public FileItem(string name)
        {
            Name = name;
        }
    }
}