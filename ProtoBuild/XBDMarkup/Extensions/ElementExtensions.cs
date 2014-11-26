using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ProtoBuild.Types;
using ProtoBuild.XBDMarkup.Elements;
using ProtoBuild.XBDMarkup.Elements.Interfaces;
using ProtoBuild.XBDMarkup.Errors;

namespace ProtoBuild.XBDMarkup
{

    internal static class ElementExtensions
    {
        private class RegisteredElement
        {
            public string Name { get; set; }
            public Type Element { get; set; }
            public RegisteredElement(string name, Type element)
            {
                this.Name = name;
                this.Element = element;
            }
        }
        static ElementExtensions()
        {
            registeredElements = new List<RegisteredElement>();
        }
        private static List<RegisteredElement> registeredElements;
        public static void RegisterElement(string name, Type element)
        {
            registeredElements.Add(new RegisteredElement(name, element));
        }
        public static void AddChild(this IXBDParentElement element, XBDElement newChild)
        {
            Type t = element.GetType();
            element.Children.Add(newChild);

        }
        public static void ParseElement(this IXBDParentElement element, XBDDef def, XmlReader reader)
        {
            XBDElement newElement = null;
            // if (element is ComplexType && ((ComplexType)element).Name == "School")
            // {
            //     newElement = null;
            //  }
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    
                        var subReader = reader.ReadSubtree();
                        subReader.Read();
                        newElement = ResolveElementType((XBDElement)element, subReader.Name, def);
                        if (newElement != null)
                        {
                            newElement.DoParse((XBDElement)element, subReader);
                            newElement.Plan = ((XBDElement)element).Plan;
                            element.AddChild(newElement);
                        }
                        else
                        {
                            def.Failed = true;
                            def.PushError(new ParseError() { Message = "Unexpected element type: " + subReader.Name, SourceLine = ((IXmlLineInfo)reader).LineNumber });
                        }
                }
            }

        }
        public static XBDElement ResolveElementType(XBDElement parent, string target, XBDDef def)
        {
            IEnumerable<RegisteredElement> elements = registeredElements.Where(x => x.Name == target);
            foreach (var element in elements)
            {

                var attrs = element.Element.GetCustomAttributes(typeof(ChildTypes), true);
                foreach (var attr in attrs)
                {
                    ChildTypes ct = (ChildTypes)attr;
                    if (ct.TypeChain == null || ct.TypeChain.Length == 0)
                    {
                            return (XBDElement)TypeHelper.GetConstructor(element.Element, new Type[] { typeof(XBDDef)})(def);
                    }
                    else if (ct.TypeChain[0] == parent.GetType())
                    {
                        if (IsValidChild(parent.GetType(), element.Element))
                        {
                            return (XBDElement)TypeHelper.GetConstructor(element.Element, new Type[] { typeof(XBDDef) })(def);
                        }
                    }
                }
            }
            return null;
        }
        private static bool IsValidChild(Type parentType, Type targetType)
        {
            var attrs = parentType.GetCustomAttributes(typeof(ChildTypes), true);
            foreach (var attr in attrs)
            {
                ChildTypes ct = (ChildTypes)attr;
                foreach (Type t in ct.Types)
                {
                    if (t == targetType)
                        return true;
                }
            }
            return false;
        }
    }
}
