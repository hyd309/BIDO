using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TableStoreConsole.Data
{
    public class Device_Location_20170307
    {
        [Key]
        public int id { get; set; }
        public long device_code { get; set; }
        public int route_id { get; set; }
        public DateTime gps_time { get; set; }
        public int latitude { get; set; }
        public int longitude { get; set; }
        public bool is_location { get; set; }
        public bool n_s { get; set; }
        public bool e_w { get; set; }
        public bool gps_or_bsl { get; set; }
        public int speed { get; set; }
        public int direct { get; set; }
        public int gps_number { get; set; }
        public int gms_signal_quality { get; set; }
        public int mileage { get; set; }
        public DateTime create_time { get; set; }
    }
}
