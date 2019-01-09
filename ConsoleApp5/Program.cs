using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ConsoleApp5 {


    internal class Serializer {
        private XmlWriter xmlw;
        private bool _SerializeInner(object obj) {
            if (obj.GetType() == typeof(string)) {
                return false;
            }
            var props = obj.GetType().GetProperties();
            if (props.Length == 0) {
                return false;
            } // Contact :| FirstName, LastName, PhoneNumber, Email |
            foreach (var prop in props) {
                var name = prop.Name; // FirstName
                this.xmlw.WriteStartElement(name);// <FirstName>
                var val = prop.GetValue(obj); // obj.FirstName
                var enumerableVal = val as IEnumerable;
                if (enumerableVal != null && typeof(string) != enumerableVal.GetType()) {
                    foreach (var item in enumerableVal) {
                        _SerializeInner(item);
                    }
                } else {
                    if (!_SerializeInner(val)) {
                        this.xmlw.WriteValue(val);
                    }
                }
                this.xmlw.WriteEndElement();
            }
            return true;
        }

        public void Serialize(object obj, string filename) {
            var opt = new XmlWriterSettings {
                Indent = true 
            };
            using (xmlw = XmlWriter.Create(filename, opt)) {
                xmlw.WriteStartDocument();                
                xmlw.WriteStartElement(nameof(obj));
                    _SerializeInner(obj); // 99% of work
                xmlw.WriteEndElement();
            }
        }
    }







    internal struct Address {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }
    internal class Product {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
    internal class OrderDetail {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
    internal class BuyerInfo {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
    internal class Order {
        public int Id { get; set; }
        public IEnumerable<OrderDetail> OrderItems { get; set; }
        public Address DeliveryAddress { get; set; }
        public BuyerInfo BuyerInfo { get; set; }
    }



    internal class Program {
         private static void Main(string[] args) {
            var obj = new Order {
                Id = 42,
                BuyerInfo = new BuyerInfo {
                    Name = "John",
                    Surname = "Doe",
                    Email = "johndoe@gmail.com",
                    Phone = "88005553535"
                },
                DeliveryAddress = new Address {
                    Street = "123 Main Street",
                    Country = "USA",
                    City = "Chicago",
                    State = "IL",
                    ZipCode = "60601"
                },
                OrderItems = new[] {
                    new OrderDetail {
                        Quantity = 1,
                        Product = new Product {
                            Id = 1003,
                            Name = "VGA TO HDMI 3m",
                            Price = 12
                        }
                    },
                    new OrderDetail {
                        Quantity = 3,
                        Product = new Product {
                            Id = 1042,
                            Name = "Flash Drive 32GB Sandisk",
                            Price = 16
                        }
                    },
                    new OrderDetail {
                        Quantity = 1,
                        Product = new Product {
                            Id = 765,
                            Name = "Captain Morgan 0.7l",
                            Price = 38
                        }
                    },
                }
            };

            var xmls = new Serializer();
            xmls.Serialize(obj, "file.xml");

            var data = File.ReadAllText("file.xml");
            System.Console.WriteLine(data);
        }
    }
}
