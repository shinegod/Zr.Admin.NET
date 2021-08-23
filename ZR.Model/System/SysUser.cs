﻿using Newtonsoft.Json;
using Npoi.Mapper.Attributes;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZR.Model.System
{
    /// <summary>
    /// 用户表
    /// </summary>
    [SugarTable("sys_user")]
    public class SysUser : SysBase
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public long UserId { get; set; }
        [Column("用户名")]//对应Excel列名
        //[Required]//校验必填
        //[Duplication]//校验模板类该列数据是否重复
        public string UserName { get; set; }

        [Column("用户昵称")]
        //[Required]
        public string NickName { get; set; }

        /// <summary>
        /// '用户类型（00系统用户）',
        /// </summary>
        //[JsonProperty(propertyName: "userType")]
        //public string User_type { get; set; } = "";
        [SugarColumn(IsOnlyIgnoreInsert = true)]
        public string Avatar { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        //[ColName("用户密码")]
        public string Password { get; set; }

        //[ColName("手机号")]
        public string Phonenumber { get; set; }

        /// <summary>
        /// 用户性别（0男 1女 2未知）
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 帐号状态（0正常 1停用）
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        [SugarColumn(IsOnlyIgnoreInsert = true)]
        public string DelFlag { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        [SugarColumn(IsOnlyIgnoreInsert = true)]
        public string LoginIP { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [SugarColumn(IsOnlyIgnoreInsert = true)]
        public DateTime LoginDate { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public long DeptId { get; set; }
        
        #region 表额外字段
        public bool IsAdmin()
        {
            return IsAdmin(UserId);
        }
        public static bool IsAdmin(long userId)
        {
            return 1 == userId;
        }

        /// <summary>
        /// 拥有角色个数
        /// </summary>
        //[SugarColumn(IsIgnore = true)]
        //public int RoleNum { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string DeptName { get; set; }
        /// <summary>
        /// 角色id集合
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int[] RoleIds { get; set; }
        /// <summary>
        /// 岗位集合
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int[] PostIds { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<SysRole> Roles { get; set; }

        #endregion
    }
}
