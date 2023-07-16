using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlibabaCloud.SDK.Alidns20150109.Models;
using AutoMapper;
using TencentCloud.Dnspod.V20210323.Models;


namespace Hua.DDNS.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DescribeDomainRecordsResponseBody.DescribeDomainRecordsResponseBodyDomainRecords.DescribeDomainRecordsResponseBodyDomainRecordsRecord, DnsRecord>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RecordId))
                .ForMember(dest => dest.RecordType, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Ip, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.SubDomain, opt => opt.MapFrom(src => src.RR))
                ;
            CreateMap<DnsRecord, UpdateDomainRecordRequest> ()
                .ForMember(dest => dest.RecordId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.RecordType))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Ip))
                .ForMember(dest => dest.RR, opt => opt.MapFrom(src => src.SubDomain))
                ;


            CreateMap<RecordListItem, DnsRecord>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RecordId))
                .ForMember(dest => dest.RecordType, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Ip, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.SubDomain, opt => opt.MapFrom(src => src.Name))
                ;
            CreateMap<DnsRecord, AddDomainRecordRequest>()
                //.ForMember(dest => dest., opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.RecordType))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Ip))
                .ForMember(dest => dest.RR, opt => opt.MapFrom(src => src.SubDomain))
                .ForMember(dest => dest.DomainName, opt => opt.MapFrom(src => src.Domain))
                ;

        }
    }
}
