﻿//using Dapper.Contrib.Extensions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZR.Model.System
{
    /// <summary>
    /// 字典类型表
    /// </summary>
    [SugarTable("sys_dict_type")]//当和数据库名称不一样可以设置别名
    //[Table("sys_dict_type")]
    public class SysDictType : SysBase
    {
        /// <summary>
        /// 字典主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//主键并且自增 （string不能设置自增）
        //[Key]
        public long DictId { get; set; }
        /// <summary>
        /// 字典名称
        /// </summary>
        public string DictName { get; set; }
        /// <summary>
        /// 字典类型
        /// </summary>
        public string DictType { get; set; }
        /// <summary>
        /// 状态 0、正常 1、停用
        /// </summary>
        public string Status { get; set; }
    }
}
