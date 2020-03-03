using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace xjplc.tanuo
{
    public class CGISDK
    {
        /******************************************************************************
  SDKCGI错误码定义，对应IPCCGI_GetLastError接口的返回值
  *******************************************************************************/
        /* 通用错误码 */
        const int ERR_SUCCEED = 0;          /**< 执行成功 */
        const int ERR_FAIL = -1;          /**< 执行失败 */

        const long ERROR_INTERNET_CANNOT_CONNECT = 12002L; //连接超时
        const long ERROR_INVALID_PARAMETER = 87L; //参数无效

        const int IPC_STATUS_OK = 200;//跟设备的请求成功
        const int IPC_STATUS_INVALID_CHANNEL = 401;//通道号不可用
        const int IPC_STATUS_INVALID_PARAMETER_VALUE = 402;//参数值不可用
        const int IPC_STATUS_INVALID_PARAMETER_NAME = 403;//参数名不可用
        const int IPC_STATUS_FEATURE_NOT_SUPPORT = 501;//服务器不支持属性
        const int IPC_STSTUS_FEATURE_NOT_AVALIABLE = 502;//服务器支持属性但在当前模式下不可用
        const int IPC_STATUS_SERVICE_FAIL = 503;//服务器过载


        /******************************************************************************
        SDKCGI获取错误码接口
        *******************************************************************************/
        /**
        * 获取错误码
        * @return 返回值为错误码
        * @note 无
        */

        [DllImport("IPCSDK_CGI.dll")]
        public static extern uint IPCCGI_GetLastError();


        /******************************************************************************
SDKCGI设备参数配置接口(CGI方式)
*******************************************************************************/
        /**
        * 获得设备的配置信息
        * @param [IN]      sUrl      CGI方式的URL，需要获取参数的字符串，如：http://192.168.112.2/cgi/major_stream_get?Channel=1&Group=StreamInfo 
        * @param [IN][OUT] sValue    返回的结果，使用XML形式的字符串
        * @param [IN]      sUsername 用户名，可为NULL
        * @param [IN]      sPassword 密码，可为NULL
        * @param [IN]      nWaittime 等待超时时间
        * @param [IN]      nPort	 HTTP端口号
        * @return 返回如下结果：
        * - 成功：true
        * - 失败：false
        * - 获取错误码调用IPCCGI_GetLastError
        * @note 无
        */
        [DllImport("IPCSDK_CGI.dll")]
        public static extern bool IPCCGI_GetValue(string sUrl,
                                        ref byte ss,
                                        string sUsername,
                                        string sPassword,
                                        uint iWaitTime = 1000,
                                        uint nPort = 80);




        /**
    * 设置设备的配置信息
    * @param [IN]      sUrl      CGI方式的URL，传入设置参数的字符串，如：http://192.168.112.2/cgi/major_stream_set?Channel=1&Group=StreamInfo 
    * @param [IN][OUT] sValue    返回的结果，使用XML形式的字符串
    * @param [IN]      sUsername 用户名，可为NULL
    * @param [IN]      sPassword 密码，可为NULL
    * @param [IN]      nWaittime 等待超时时间
    * @param [IN]      nPort	 HTTP端口号
    * @return 返回如下结果：
    * - 成功：true
    * - 失败：false
    * - 获取错误码调用IPCCGI_GetLastError
    * @note 无
    */
    [DllImport("IPCSDK_CGI.dll")]
    public static extern bool IPCCGI_SetValue(ref byte ss0,
                                               ref byte ss,
                                              string  sUsername,
                                              string sPassword,
                                              uint iWaitTime = 500,
                                              uint nPort = 80);
        /**
    * 查询设备的配置信息
    * @param [IN]      sUrl      CGI方式的URL，传入查询参数的字符串，如：http://192.168.112.2/cgi/major_stream_query?Group=GroupName  
    * @param [IN][OUT] sValue    返回的结果，使用XML形式的字符串
    * @param [IN]      sUsername 用户名，可为NULL
    * @param [IN]      sPassword 密码，可为NULL
    * @param [IN]      nWaittime 等待超时时间
    * @param [IN]      nPort	 HTTP端口号
    * @return 返回如下结果：
    * - 成功：true
    * - 失败：false
    * - 获取错误码调用IPCCGI_GetLastError
    * @note 无
    */
        [DllImport("IPCSDK_CGI.dll")]
        public static extern bool IPCCGI_QueryValue(string sUrl,
                                          string sValue,
                                          string sUsername,
                                          string sPassword,
                                          uint iWaitTime = 500,
                                          uint nPort = 80);

        /**
        * 获取通用的数据，即查询后返回的数据，与IPCCGI_QueryValue，IPCCGI_GetValue是同样的方式，主要不同在于，可以得到数据缓冲的指针地址
        * 和大小，以便处理非字符串类型的数据，比如图片，报警等.
        * @param [IN]      sUrl      CGI方式的URL，传入查询参数的字符串，如：http://192.168.112.2/cgi/major_stream_query?Group=GroupName  
        * @param [IN][OUT] pBuf      结果数据的缓冲指针
        * @param [IN]      sUsername 用户名，可为NULL
        * @param [IN]      sPassword 密码，可为NULL
        * @param [IN]      nWaitTime 等待超时时间
        * @param [IN]      nPort	 HTTP端口号
        * @return 返回如下结果：
        * 结果数据缓冲大小,大于0表示返回了有效数据
        * - 获取错误码调用IPCCGI_GetLastError
        * @note 无
        **/
        [DllImport("IPCSDK_CGI.dll")]
        public static extern UInt32  IPCCGI_GetCommonData(
                                             string sUrl,
                                             string pBuf,
                                             string sUsername,
                                             string sPassword,
                                             uint iWaitTime = 500,
                                             uint nPort = 80);

    }

}
