﻿using ProjectEye.Core.Models.Options;
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
        public ProjectEye.Core.Models.Options.OptionsModel Data
        {
            get
            {
                return data_;
            }
            set
            {
                data_ = value;
                OnPropertyChanged("Data");
            }
        }

        private string version_;
        public string Version
        {
            get
            {
                return version_;
            }
            set
            {
                version_ = value;
                OnPropertyChanged("Version");
            }
        }

        public string VersionLink
        {
            get
            {
                return "https://github.com/Planshit/ProjectEye/releases/tag/" + Version;
            }

        }

        public List<ThemeModel> Themes { get; set; }
    }
}