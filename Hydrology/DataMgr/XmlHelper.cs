/************************************************************************************
* Copyright (c) 2019 All Rights Reserved.
*命名空间：DBManager
*文件名： XmlHelper
*创建人： XXX
*创建时间：2019-2-26 19:28:56
*描述
*=====================================================================
*修改标记
*修改时间：2019-2-26 19:28:56
*修改人：XXX
*描述：
************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Hydrology.DataMgr
{
    public class XmlHelper
    {
        
        public static string getXMLInfo()
        {
            //将XML文件加载进来
            string result = string.Empty;
            XDocument document = XDocument.Load("config\\zhongaopath.xml");
            //获取到XML的根元素进行操作
            XElement root = document.Root;
            XElement path = root.Element("path");
            //获取name标签的值
            result = path.Value.ToString();
            return result;
            
        }
    }
}