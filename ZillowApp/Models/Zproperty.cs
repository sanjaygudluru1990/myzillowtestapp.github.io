using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZillowApp.Models
{
    public class ZException
    {
        public int StatusCode { get; set; }
        public string ErrorDescription { get; set; }
    }
    public class Zproperty
    {
        public int ZipId { get; set; }
        public Zlinks Zlinks { get; set; }
        public Address Address { get; set; }
        public Zestimate Zestimate { get; set; }
        public List<LocalNeighbourHood> LocalNeighbourHoods { get; set; }
    }

    public class Zlinks
    {
        public string HomeDetails { get; set; }
        public string GraphSandData { get; set; }
        public string MapthisHome { get; set; }
        public string Comparables { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Zestimate
    {
        public int Amount { get; set; }
        public string LastUpdated { get; set; }
        public bool IsDeprecated { get; set; }
        public int maxValue { get; set; }
        public int minValue { get; set; }
        public PropertyValueChange PropertyValueChange { get; set; }
    }

    public class LocalNeighbourHood
    {
        public string Type { get; set; }
        public int NeighbourHoodId { get; set; }
        public string NeighbourHoodName { get; set; }
        public string ZIndexValue { get; set; }
        public NeighbourHoodRegionLinks NeighbourHoodRegionLinks { get; set; }
    }

    public class NeighbourHoodRegionLinks
    {
        public string OverView { get; set; }
        public string ForSaleByOwner { get; set; }
        public string forSale { get; set; }
    }

    public class PropertyValueChange
    {
        public string Currency { get; set; }
        public int Duration { get; set; }
        public int PriceDeviation { get; set; }
    }
}