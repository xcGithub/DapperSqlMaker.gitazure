
using System;
using Dapper;
using Dapper.Contrib.Extensions;

namespace FW.Model
{

    /// <summary>
    /// A class which represents the LockPers table.
    /// </summary>
	public partial class LockPers
	{
        /*  Name  Content  Prompt  Id  InsertTime  IsDel  DelTime  */

        //public virtual string Name { get; set; }
        //public virtual string Content { get; set; }
        //      // this.OnPropertyValueChange()  
        //      public virtual string Prompt { get; set; }
        //      �ֶ���Ϊid��Ĭ�ϵ�������   [ExplicitKey]  // ��ʾ�� �����и�ֵ
        //public virtual string Id { get; set; }
        //public virtual DateTime? InsertTime { get; set; }
        //public virtual string IsDel { get; set; }
        //public virtual DateTime? DelTime { get; set; }
        //      public virtual DateTime? UpdateTime { get; set; }
         
        //public LockPers() {
        //}  //this._IsWriteFiled = false; }  

        

        private string _ContentOld;
        [Write(false)]
        public virtual string ContentOld {
            get { return _ContentOld; }
            set {
                _ContentOld = value;
                // _WriteFiled.Add(this.GetType().GetProperty("ContentOld") );  // �ǿ��ֶ������¼
            }
        }
        [Write(false)]
        public Users Users { get; set; }

    }
     
} // namespace
