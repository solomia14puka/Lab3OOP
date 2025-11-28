using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lab3OOP.Model
{
    public class Contact : INotifyPropertyChanged
    {
        private string _fullName = string.Empty;
        private string _faculty = string.Empty;
        private string _department = string.Empty;
        private string _specialty = string.Empty;
        private string _collaboration = string.Empty;
        private string _timeFrame = string.Empty;

        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        public string Faculty
        {
            get => _faculty;
            set { _faculty = value; OnPropertyChanged(); }
        }

        public string Department
        {
            get => _department;
            set { _department = value; OnPropertyChanged(); }
        }

        public string Specialty
        {
            get => _specialty;
            set { _specialty = value; OnPropertyChanged(); }
        }

        public string Collaboration
        {
            get => _collaboration;
            set { _collaboration = value; OnPropertyChanged(); }
        }

        public string TimeFrame
        {
            get => _timeFrame;
            set { _timeFrame = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
