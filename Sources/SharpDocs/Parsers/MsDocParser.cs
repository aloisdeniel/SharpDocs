namespace SharpDocs.Parsers
{
    using Entities;
    using System.Xml.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;

    public class MsDocParser
    {
        private Content ParseContent(XElement element)
        {
            if(element == null)
            {
                return null;
            }

            var result = new Content();
            
            foreach (var n in element.Nodes())
            {
                if(n.NodeType == XmlNodeType.Text)
                {
                    result.Add(new Content.Text() { Value = n.ToString() });
                }
                else if(n.NodeType == XmlNodeType.Element)
                {
                    var e = n as XElement;
                    if (e.Name == "c")
                    {
                        result.Add(new Content.InlineCode()
                        {
                            Value = e.Value,
                        });
                    }
                    else if(e.Name == "code")
                    {
                        result.Add(new Content.Code()
                        {
                            Value = e.Value,
                        });
                    }
                    else if (e.Name == "paramref")
                    {
                        result.Add(new Content.ParameterReference()
                        {
                            Value = e.Attribute("name")?.Value,
                        });
                    }
                    else if (e.Name == "typeparamref")
                    {
                        result.Add(new Content.TypeParameterReference()
                        {
                            Value = e.Attribute("name")?.Value,
                        });
                    }
                    else if (e.Name == "see")
                    {
                        result.Add(new Content.See()
                        {
                            Reference = e.Attribute("cref")?.Value,
                        });
                    }
                    else if (e.Name == "list")
                    {
                        var listitems = new List<Content.List.Item>();
                        var list = new Content.List()
                        {
                            Type = e.Attribute("type")?.Value,
                            Items = listitems,
                        };

                        foreach (var item in e.Elements("item"))
                        {
                            listitems.Add(new Content.List.Item()
                            {
                                Value = ParseContent(item.Element("description")),
                            });
                        }

                        result.Add(list);
                    }
                }
                
            }

            return result;
        }

        public Documentation Parse(string xmlDocFile)
        {
            if (File.Exists(xmlDocFile))
            {
                var xml = XDocument.Load(xmlDocFile).Root;

                var result = new Documentation()
                {
                    Assembly = new Member() {
                        Name = xml.Element("assembly")?.Element("name")?.Value,
                    },
                };

                var members = new List<Member>();

                foreach (var nMember in xml.Element("members")?.Elements("member"))
                {
                    var member = new Member()
                    {
                        Name = nMember.Attribute("name")?.Value,
                        Summary = ParseContent(nMember.Element("summary")),
                        Returns = ParseContent(nMember.Element("returns")),
                        Example = ParseContent(nMember.Element("example")),
                        Remarks = ParseContent(nMember.Element("remarks")),
                        TypeParameters = nMember.Elements("typeparam")?.Select((n) => new Parameter()
                        {
                            Name = n.Attribute("name")?.Value,
                            Reference = n.Attribute("cref")?.Value,
                            Summary = ParseContent(n),
                        }),
                        Parameters = nMember.Elements("param")?.Select((n) => new Parameter()
                        {
                            Name = n.Attribute("name")?.Value,
                            Reference = n.Attribute("cref")?.Value,
                            Summary = ParseContent(n),
                        }),
                        Exceptions = nMember.Elements("exception")?.Select((n) => new Exception()
                        {
                            Reference = n.Attribute("cref")?.Value,
                            Summary = ParseContent(n),
                        }),
                    };

                    members.Add(member);
                }

                result.Members = members;
                
                return result;
            }

            throw new FileNotFoundException("File not found : " + xmlDocFile);
        }
    }
}
