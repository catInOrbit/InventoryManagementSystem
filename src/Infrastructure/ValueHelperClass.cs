﻿// using System.Reflection;
//
// namespace Infrastructure
// {
//     public static class ValueHelperClass : IAggregateRoot 
//     {
//         public static void CopyProperties(this EntityBase source, EntityBase destination)
//         {
//             // Iterate the Properties of the destination instance and  
//             // populate them from their source counterparts  
//             PropertyInfo[] destinationProperties = destination.GetType().GetProperties(); 
//             foreach (PropertyInfo destinationPi in destinationProperties)
//             {
//                 PropertyInfo sourcePi = source.GetType().GetProperty(destinationPi.Name);     
//                 destinationPi.SetValue(destination, sourcePi.GetValue(source, null), null);
//             } 
//         }
//     }
// }