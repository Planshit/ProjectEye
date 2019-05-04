using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ProjectEye.Models
{
    public class OptionsModel : UINotifyPropertyChanged
    {
        private ProjectEye.Core.Models.Options.OptionsModel data_;
        public ProjectEye.Core.Models.Options.OptionsModel data
        {
            get
            {
                return data_;
            }
            set
            {
                data_ = value;
                OnPropertyChanged("data");
            }
        }

        private string version_;
        public string version
        {
            get
            {
                return version_;
            }
            set
            {
                version_ = value;
                OnPropertyChanged("version");
            }
        }

        public string versionlink
        {
            get
            {
                return "https://github.com/Planshit/ProjectEye/releases/tag/" + version;
            }

        }

    }
}
