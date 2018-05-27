namespace FrgxPublicApiSDK.Models
{
    /// <summary>
    /// Object to hold information for a shipping address
    /// </summary>
    public class ShippingAddress
    {
        /// <summary>
        /// First name
        /// </summary>        
        public string FirstName { get; set; }
        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Address 1
        /// </summary>
        public string Address1 { get; set; }
        /// <summary>
        /// Address 2
        /// </summary>
        public string Address2 { get; set; }
        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// State
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Zipcode
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Returns a string with all ShippingAddress information
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            return "First Name : " + FirstName + "\nLast Name : " + LastName + "\nAddress 1 : " + Address1 + "\nAddress 2 : " + Address2 + "\nCity : " + City + "\nState : " + State + "\nZipcode : " + ZipCode + "\nCountry : " + Country + "\nPhone : " + Phone;
        }
    }
}
