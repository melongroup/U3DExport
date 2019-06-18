using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace foundation
{
    class AMF3NameObjectCollectionWriter : IAMFWriter
    {
		public AMF3NameObjectCollectionWriter()
		{
		}
		
        #region IAMFWriter Members

		public bool IsPrimitive{ get{return false;} }

        public void WriteData(AMFWriter writer, object data)
        {
            NameObjectCollectionBase collection = data as NameObjectCollectionBase;
            object[] attributes = collection.GetType().GetCustomAttributes(typeof(DefaultMemberAttribute), false);
            if (attributes != null  && attributes.Length > 0)
            {
                DefaultMemberAttribute defaultMemberAttribute = attributes[0] as DefaultMemberAttribute;
                PropertyInfo pi = collection.GetType().GetProperty(defaultMemberAttribute.MemberName, new Type[] { typeof(string) });
                if (pi != null)
                {
                    ASObject aso = new ASObject();

                    IEnumerator enumList = collection.Keys.GetEnumerator();

                    //Ϊ�˼��� webplayer ���ֻ� platform ���ĵ��Ĵ��룬ԭ���Ĵ���������ע������.
                    while (enumList.MoveNext())
                    {
                        string key = enumList.Current as string;
                        object value = pi.GetValue(collection, new object[] { key });
                
                        aso.Add(key, value);
                    }
                    writer.WriteByte(AMF3TypeCode.Object);
                    writer.WriteAMF3Object(aso);
                    return;
                }
            }

            //We could not access an indexer so write out as it is.
            writer.WriteByte(AMF3TypeCode.Object);
            writer.WriteAMF3Object(data);
        }

        #endregion
    }
}
