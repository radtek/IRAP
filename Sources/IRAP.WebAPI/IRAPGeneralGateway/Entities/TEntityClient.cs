﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRAPGeneralGateway.Entities
{
    public class TEntityClient
    {
        /// <summary>
        /// 客户端标识
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ChannelName { get; set; }
        /// <summary>
        /// 加密密钥
        /// </summary>
        public string SecurityPassword { get; set; }
        /// <summary>
        /// 安全级别：1:-明文；2-压缩；3-DES加密及GZip压缩；4-AES加密
        /// </summary>
        public int SecurityLevel { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public int IsValid { get; set; }
        public DateTime ExpireDate { get; set; }
        public string AuthLibrary { get; set; }
    }
}
