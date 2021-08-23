﻿using System;
using System.Collections.Generic;
using System.Text;
using ZR.Model.System;

namespace ZR.Service.IService
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISysDictService: IBaseService<SysDictType>
    {
        public List<SysDictType> SelectDictTypeList(SysDictType dictType);

        /// <summary>
        /// 根据Id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SysDictType SelectDictTypeById(long id);

        /// <summary>
        /// 校验字典类型称是否唯一
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <returns></returns>
        public string CheckDictTypeUnique(SysDictType dictType);

        /// <summary>
        /// 删除一个
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteDictTypeById(long id);

        /// <summary>
        /// 批量删除字典数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteDictTypeByIds(long[] dictIds);

        /// <summary>
        /// 插入字典类型
        /// </summary>
        /// <param name="sysDictType"></param>
        /// <returns></returns>
        public long InsertDictType(SysDictType sysDictType);

        /// <summary>
        /// 修改字典类型
        /// </summary>
        /// <param name="sysDictType"></param>
        /// <returns></returns>
        public int UpdateDictType(SysDictType sysDictType);
    }
}
