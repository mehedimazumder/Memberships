﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Memberships.Models
{
    public class ContentViewModel
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Html { get; set; }
        public string VideoUrl { get; set; }
    }
}