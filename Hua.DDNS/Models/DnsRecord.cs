﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hua.DDNS.Models;

public class DnsRecord
{
    public string Id { get; set; }
    public string Ip { get; set; }
    public string Host { get; set; }
    public string SubDomain { get; set; }
    public string Domain { get; set; }
    public string TTL { get; set; } = "10";
    public string RecordType { get; set; } = "A";

    public DnsRecord(string ip,string domain)
    {
        Ip = ip;
        Host = domain;
    }

    public DnsRecord()
    {

    }
}