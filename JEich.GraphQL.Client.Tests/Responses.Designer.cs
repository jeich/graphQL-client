﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JEich.GraphQL.Tests {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Responses {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Responses() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("JEich.GraphQL.Tests.Responses", typeof(Responses).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;data&quot;: {
        ///    &quot;Obi-wan Kanobi&quot;: {
        ///      &quot;name&quot;: &quot;Obi-wan Kanobi&quot;,
        ///      &quot;friends&quot;: [{
        ///          &quot;name&quot;: &quot;Luke Skywalker&quot;
        ///        }]
        ///    },
        ///	&quot;Qui-gon Xin&quot;: {
        ///		&quot;name&quot;: &quot;Qui-gon Xin&quot;
        ///	}
        ///  }
        ///}.
        /// </summary>
        public static string AliasedObjects {
            get {
                return ResourceManager.GetString("AliasedObjects", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;data&quot;: {
        ///    &quot;hero&quot;: {
        ///      &quot;name&quot;: &quot;R2-D2&quot;
        ///    }
        ///  }
        ///}.
        /// </summary>
        public static string Basic {
            get {
                return ResourceManager.GetString("Basic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;data&quot;: {
        ///    &quot;lonelyhero&quot;: {
        ///      &quot;name&quot;: &quot;R2-D2&quot;,
        ///      &quot;friend&quot;: {
        ///          &quot;name&quot;: &quot;Luke Skywalker&quot;
        ///        }
        ///    },
        ///	&quot;hero&quot;: {
        ///		&quot;name&quot;: &quot;Obi-wan Kanobi&quot;
        ///	}
        ///  }
        ///}.
        /// </summary>
        public static string MultipleObjects {
            get {
                return ResourceManager.GetString("MultipleObjects", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;data&quot;: {
        ///    &quot;hero&quot;: {
        ///      &quot;name&quot;: &quot;R2-D2&quot;,
        ///      &quot;friends&quot;: [
        ///        {
        ///          &quot;name&quot;: &quot;Luke Skywalker&quot;
        ///        },
        ///        {
        ///          &quot;name&quot;: &quot;Han Solo&quot;
        ///        },
        ///        {
        ///          &quot;name&quot;: &quot;Leia Organa&quot;
        ///        }
        ///      ]
        ///    }
        ///  }
        ///}.
        /// </summary>
        public static string NestedArray {
            get {
                return ResourceManager.GetString("NestedArray", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;errors&quot;: [
        ///    {
        ///      &quot;message&quot;: &quot;Name for character with ID 1002 could not be fetched.&quot;,
        ///      &quot;locations&quot;: [ { &quot;line&quot;: 6, &quot;column&quot;: 7 } ],
        ///      &quot;path&quot;: [ &quot;hero&quot;, &quot;heroFriends&quot;, 1, &quot;name&quot; ]
        ///    }
        ///  ],
        ///  &quot;data&quot;: {
        ///    &quot;hero&quot;: {
        ///      &quot;name&quot;: &quot;R2-D2&quot;,
        ///      &quot;friends&quot;: [
        ///        {
        ///          &quot;id&quot;: &quot;1000&quot;,
        ///          &quot;name&quot;: &quot;Luke Skywalker&quot;
        ///        },
        ///        null,
        ///        {
        ///          &quot;id&quot;: &quot;1003&quot;,
        ///          &quot;name&quot;: &quot;Leia Organa&quot;
        ///        }
        ///      ]
        ///    }
        ///  }
        ///}.
        /// </summary>
        public static string NestedArrayWithErrors {
            get {
                return ResourceManager.GetString("NestedArrayWithErrors", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;data&quot;: {
        ///    &quot;lonelyhero&quot;: {
        ///      &quot;name&quot;: &quot;R2-D2&quot;,
        ///      &quot;friend&quot;: {
        ///          &quot;name&quot;: &quot;Luke Skywalker&quot;
        ///        }
        ///    }
        ///  }
        ///}.
        /// </summary>
        public static string NestedObject {
            get {
                return ResourceManager.GetString("NestedObject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;data&quot;: {
        ///    //Comment
        ///    &quot;lonelyhero&quot;: {
        ///      &quot;name&quot;: &quot;R2-D2&quot;,
        ///      &quot;friend&quot;: {
        ///          &quot;name&quot;: &quot;Luke Skywalker&quot;
        ///        }
        ///    }
        ///  }
        ///}.
        /// </summary>
        public static string NestedObjectWithComment {
            get {
                return ResourceManager.GetString("NestedObjectWithComment", resourceCulture);
            }
        }
    }
}
