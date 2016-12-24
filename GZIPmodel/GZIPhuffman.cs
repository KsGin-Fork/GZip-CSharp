using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace GZIPmodel
{
    /// <summary>
    /// GZIP核心类,使用huffman编码
    /// </summary>
    internal class GZIPhuffman
    {

    //--------------------------------------------------------数据类型定义------------------------------------------------------------
        /// <summary>
        /// 节点类定义
        /// </summary>
        internal class GZIPhuffmanNode
        {
            /// <summary>
            /// 节点字节属性
            /// </summary>
            internal char Value { get; set; }

            /// <summary>
            /// 双亲节点
            /// </summary>
            internal GZIPhuffmanNode mPNode;

            /// <summary>
            /// 左子节点
            /// </summary>
            internal readonly GZIPhuffmanNode mLNode;

            /// <summary>
            /// 右子节点
            /// </summary>
            internal readonly GZIPhuffmanNode mRNode;

            /// <summary>
            /// 构造方法
            /// </summary>
            /// <param name="mPNode">双亲节点</param>
            /// <param name="mLNode">左孩子节点</param>
            /// <param name="mRNode">右孩子节点</param>
            public GZIPhuffmanNode(GZIPhuffmanNode mPNode, GZIPhuffmanNode mLNode, GZIPhuffmanNode mRNode)
            {
                this.mPNode = mPNode;
                this.mLNode = mLNode;
                this.mRNode = mRNode;
            }
        }

        /// <summary>
        /// Huffman树
        /// </summary>
        private readonly List<GZIPhuffmanNode> mHuffmaGziPhuffmanNodes = new List<GZIPhuffmanNode>();

        /// <summary>
        /// 建立数据和字节码对照表
        /// </summary>
        private readonly Dictionary<char,string> mTransTable = new Dictionary<char, string>();

        /// <summary>
        /// 数据字典 key:数据 value:权值
        /// </summary>
        private Dictionary<char, uint> mValueWeight;

    //--------------------------------------------------------private方法定义--------------------------------------------------------------

        /// <summary>
        /// 分析字符串将其存为数据和频度对应的字典
        /// </summary>
        /// <param name="buf"></param>
        private void ParseCharArrays(string buf)
        {
            mValueWeight = new Dictionary<char, uint>();
            foreach (var data in buf)
            {
                if (mValueWeight.ContainsKey(data))         //如果值已经在字典中出现 ， 则将其频度自增1
                {
                    ++mValueWeight[data];
                }
                else                                        //未出现则存入
                {
                    mValueWeight.Add(data, 1);
                }             
            }
        }

        /// <summary>
        /// 通过字典初始化树
        /// </summary>
        private void CreatHuffman()
        {
            var isCreated = new List<bool>();        //初始化未创建列表
            var WeightList = new List<uint>(mValueWeight.Count);
            WeightList.AddRange(mValueWeight.Values);                  //初始化权值列表
            for (var i = 0; i < mValueWeight.Count; i++)                  
            {
                isCreated.Add(false);
            }
            foreach (var data in mValueWeight)
            {
                //初始化一个左右子树和双亲节点都为空的节点加入到huffmanNodes列表中
                mHuffmaGziPhuffmanNodes.Add(new GZIPhuffmanNode(null , null , null) {Value = data.Key});
            }
            CreatHuffmanNode(isCreated , WeightList);                               //递归创建huffman树
            SetTransTable();                                                        //设置对照字典
        }

        /// <summary>
        /// 递归创建huffman树节点
        /// </summary>
        private void CreatHuffmanNode(List<bool> isCreated, List<uint> WeightList)
        {
            while (true)
            {
                var fMinIndex = IndexOfWeightMin(isCreated, WeightList);
                if (fMinIndex == -1) return;
                isCreated[fMinIndex] = true;
                var sMinIndex = IndexOfWeightMin(isCreated, WeightList);
                if (sMinIndex == -1) return;
                isCreated[sMinIndex] = true;
                var newNode = new GZIPhuffmanNode(null, mHuffmaGziPhuffmanNodes[fMinIndex], mHuffmaGziPhuffmanNodes[sMinIndex]);
                //isCreated里添加新项
                isCreated.Add(false);
                //weightlist里添加新项
                WeightList.Add(WeightList[fMinIndex] + WeightList[sMinIndex]);
                //设置fMin 和 sMin的双亲节点
                mHuffmaGziPhuffmanNodes[fMinIndex].mPNode = newNode;
                mHuffmaGziPhuffmanNodes[sMinIndex].mPNode = newNode;
                //mHuffmaGziPhuffmanNodes里添加新项
                mHuffmaGziPhuffmanNodes.Add(newNode);
                //递归处理
            }
        }

        /// <summary>
        /// 返回当前列表中未被选择过的最小权值的数据索引
        /// </summary>
        /// <returns></returns>
        private int IndexOfWeightMin(IReadOnlyList<bool> isCreated , IReadOnlyList<uint> WeightList)
        {
            var indexOfMinValue = -1;
            for (var i = 0; i < mHuffmaGziPhuffmanNodes.Count ; i++)
            {
                if (isCreated[i]) continue;
                if (indexOfMinValue == -1)
                {
                    indexOfMinValue = i;
                }
                else if(WeightList[i] < WeightList[indexOfMinValue])
                {
                    indexOfMinValue = i;
                }
            }
            return indexOfMinValue;
        }

        /// <summary>
        /// 获得单个字符的编码值
        /// </summary>
        /// <param name="cj">字符</param>
        /// <returns>编码值</returns>
        private string CharToCode(char cj)
        {
            
            var result = new StringBuilder();
            
            var index = 0;
            for (var i = 0; i < (mHuffmaGziPhuffmanNodes.Count + 1) / 2; i++)
            {
                if (cj == mHuffmaGziPhuffmanNodes[i].Value)
                {
                    index = i;
                }
            }
            var curNode = mHuffmaGziPhuffmanNodes[index];
            while (curNode.mPNode != null)
            {
                if (curNode.mPNode.mLNode != null && curNode == curNode.mPNode.mLNode)
                {
                    result.Insert(0 , '0');
                }
                else
                {
                    result.Insert(0, '1');
                }
                curNode = curNode.mPNode;
            }
            return result.ToString();
        }

        /// <summary>
        /// 设置解压码和数据对照表
        /// </summary>
        private void SetTransTable()
        {
            foreach (var v in mValueWeight)
            {
                mTransTable.Add(v.Key , CharToCode(v.Key));
            }
        }

        //--------------------------------------------------------public方法定义---------------------------------------------------------------
        /// <summary>
        /// 使用字符串构造huffman树
        /// </summary>
        public GZIPhuffman(string buf)
        {
            ParseCharArrays(buf);          //通过传入参数初始化字典
            CreatHuffman();                //通过字典初始化树
        }

        /// <summary>
        /// 使用字典构造huffman树
        /// </summary>
        /// <param name="dictionary"></param>
        public GZIPhuffman(Dictionary<char, uint> dictionary)
        {
            mValueWeight = dictionary;
            CreatHuffman();
        }

        /// <summary>
        /// 字符串编码
        /// </summary>
        /// <param name="buf">要编码的字符串</param>
        /// <param name="parm">编码参数的引用传递</param>
        /// <returns>编码</returns>
        public string GZIPcoding(string buf , ref Dictionary<char , uint> parm)
        {
            var lb = new List<char>(buf);
            if (parm == null) throw new ArgumentNullException(nameof(parm));
            parm = mValueWeight;
            var sb = new StringBuilder();          
            lb.ForEach(e =>
            {
                sb.Append(mTransTable[e]);
            });
            return sb.ToString();

        }

        /// <summary>
        /// 编码解码
        /// </summary>
        /// <param name="code">编码值</param>
        /// <returns>字符串</returns>
        public string GZIPtranslate(string code)
        {
            var lc = new List<char>(code);
            var re = new StringBuilder();
            var Ky = new List<char>(mTransTable.Keys);
            var Vy = new List<string>(mTransTable.Values);
            var tmp = new StringBuilder();

            lc.ForEach(e =>                                       
            {
                int index;
                if ((index = Vy.IndexOf(tmp.ToString())) >= 0)
                {
                    re.Append(Ky[index]);
                    tmp.Clear();
                }
                tmp.Append(e); 
            });
                                    
            return re.ToString();
        }
    }
}
