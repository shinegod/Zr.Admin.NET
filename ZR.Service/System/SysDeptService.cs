﻿using Infrastructure;
using Infrastructure.Attribute;
using Infrastructure.Extensions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZR.Common;
using ZR.Model.System;
using ZR.Model.Vo.System;
using ZR.Repository.System;
using ZR.Service.IService;

namespace ZR.Service.System
{
    /// <summary>
    /// 部门管理
    /// </summary>
    [AppService(ServiceType = typeof(ISysDeptService), ServiceLifetime = LifeTime.Transient)]
    public class SysDeptService : BaseService<SysDept>, ISysDeptService
    {
        public SysDeptRepository DeptRepository;

        public SysDeptService(SysDeptRepository deptRepository)
        {
            DeptRepository = deptRepository;
        }

        /// <summary>
        /// 查询部门管理数据
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public List<SysDept> GetSysDepts(SysDept dept)
        {
            //开始拼装查询条件
            var predicate = Expressionable.Create<SysDept>();
            predicate = predicate.And(it => it.DelFlag == "0");
            predicate = predicate.AndIF(dept.DeptName.IfNotEmpty(), it => it.DeptName.Contains(dept.DeptName));
            predicate = predicate.AndIF(dept.Status.IfNotEmpty(), it => it.Status == dept.Status);

            var response = GetWhere(predicate.ToExpression());

            return response;
        }

        /// <summary>
        /// 校验部门名称是否唯一
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public string CheckDeptNameUnique(SysDept dept)
        {
            long deptId = dept.DeptId == 0 ? -1L : dept.DeptId;
            SysDept info = GetFirst(it => it.DeptName == dept.DeptName && it.ParentId == dept.ParentId);
            if (info != null && info.DeptId != deptId)
            {
                return UserConstants.NOT_UNIQUE;
            }
            return UserConstants.UNIQUE;
        }

        /// <summary>
        /// 新增保存部门信息
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public int InsertDept(SysDept dept)
        {
            SysDept info = GetFirst(it => it.DeptId == dept.ParentId);
            //如果父节点不为正常状态,则不允许新增子节点
            if (!UserConstants.DEPT_NORMAL.Equals(info.Status))
            {
                throw new CustomException("部门停用，不允许新增");
            }

            dept.Ancestors = info.Ancestors + "," + dept.ParentId;
            return Add(dept);
        }

        /// <summary>
        /// 修改保存部门信息
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public int UpdateDept(SysDept dept)
        {
            SysDept newParentDept = GetFirst(it => it.ParentId == dept.ParentId);
            SysDept oldDept = GetFirst(m => m.DeptId == dept.DeptId);
            if (newParentDept != null && oldDept != null)
            {
                string newAncestors = newParentDept.Ancestors + "," + newParentDept.DeptId;
                string oldAncestors = oldDept.Ancestors;
                dept.Ancestors = newAncestors;
                UpdateDeptChildren(dept.DeptId, newAncestors, oldAncestors);
            }
            int result = Update(dept);
            if (UserConstants.DEPT_NORMAL.Equals(dept.Status))
            {
                // 如果该部门是启用状态，则启用该部门的所有上级部门
                //UpdateParentDeptStatus(dept);
            }
            return result;
        }

        /// <summary>
        /// 修改该部门的父级部门状态
        /// </summary>
        /// <param name="dept">当前部门</param>
        private void UpdateParentDeptStatus(SysDept dept)
        {
            string updateBy = dept.Update_by;
            dept = GetFirst(it => it.DeptId == dept.DeptId);
            dept.Update_by = updateBy;
            //DeptRepository.UpdateParentDeptStatus(dept);
        }

        /// <summary>
        /// 修改子元素关系
        /// </summary>
        /// <param name="deptId">被修改的部门ID</param>
        /// <param name="newAncestors">新的父ID集合</param>
        /// <param name="oldAncestors">旧的父ID集合</param>
        public void UpdateDeptChildren(long deptId, string newAncestors, string oldAncestors)
        {
            List<SysDept> children = GetChildrenDepts(GetSysDepts(new SysDept()), deptId);

            foreach (var child in children)
            {
                child.Ancestors = child.Ancestors.Replace(oldAncestors, newAncestors);

                if (child.DeptId.Equals(deptId))
                {
                    DeptRepository.UdateDeptChildren(child);
                }
            }
        }

        public List<SysDept> GetChildrenDepts(List<SysDept> depts, long deptId)
        {
            return depts.FindAll(delegate (SysDept item)
            {
                long[] pid = Tools.SpitLongArrary(item.Ancestors);
                return pid.Contains(deptId);
            });
        }

        /// <summary>
        /// 构建前端所需要树结构
        /// </summary>
        /// <param name="depts">部门列表</param>
        /// <returns></returns>
        public List<SysDept> BuildDeptTree(List<SysDept> depts)
        {
            List<SysDept> returnList = new List<SysDept>();
            List<long> tempList = depts.Select(f => f.DeptId).ToList();
            foreach (var dept in depts)
            {
                // 如果是顶级节点, 遍历该父节点的所有子节点
                if (!tempList.Contains(dept.ParentId))
                {
                    RecursionFn(depts, dept);
                    returnList.Add(dept);
                }
            }

            if (!returnList.Any())
            {
                returnList = depts;
            }
            return returnList;
        }

        /// <summary>
        /// 构建前端所需下拉树结构
        /// </summary>
        /// <param name="depts"></param>
        /// <returns></returns>
        public List<TreeSelectVo> BuildDeptTreeSelect(List<SysDept> depts)
        {
            List<SysDept> menuTrees = BuildDeptTree(depts);
            List<TreeSelectVo> treeMenuVos = new List<TreeSelectVo>();
            foreach (var item in menuTrees)
            {
                treeMenuVos.Add(new TreeSelectVo(item));
            }
            return treeMenuVos;
        }

        /// <summary>
        /// 递归列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="t"></param>
        private void RecursionFn(List<SysDept> list, SysDept t)
        {
            //得到子节点列表
            List<SysDept> childList = GetChildList(list, t);
            t.children = childList;
            foreach (var item in childList)
            {
                if (GetChildList(list, item).Count() > 0)
                {
                    RecursionFn(list, item);
                }
            }
        }
        /// <summary>
        /// 递归获取子菜单
        /// </summary>
        /// <param name="list">所有菜单</param>
        /// <param name="dept"></param>
        /// <returns></returns>
        private List<SysDept> GetChildList(List<SysDept> list, SysDept dept)
        {
            return list.Where(p => p.ParentId == dept.DeptId).ToList();
        }
    }
}
