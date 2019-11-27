using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace xjplc
{
    public class HDVPLAYSDK
    {

        [DllImport(@"HDVSDK_Play.dll")]
        public static extern Int32 HDVPLAY_OpenStream(IntPtr pStreamHead,
                                           UInt32 lStreamHeadSize);

        /**
        * @enum tagFrametype
        * @brief 回调解码后的数据帧类型
        * @attention 暂时只提供YUV422的uyvy排列格式
*/
        public enum tagFrametype
        {
            FT_UYVY,    /**< 视频，uyvy格式。排列顺序 "U0Y0V0Y1U2Y2V2Y3……"，第一个像素位于图像左上角 */
        }

        /******************************************************************************
SDKPLAY解码控制接口
*******************************************************************************/
        /**
        * 播放开始
        * @param [IN]   lPlayHandle 解码句柄
        * @param [IN]   hPlayWnd    播放窗口句柄，为空(NULL)表示只解码不显示
        * @param [IN]   bCloseSound 仅实时播放时有效,该参数控制实时播放的同时是否关闭声音，默认是开启声音的
        * @return 返回如下结果：
        * - 成功：true
        * - 失败：false
        * - 获取错误码调用HDVPLAY_GetLastError
        * @note 播放视频画面大小将根据 hWnd 窗口调整。
        * 如果已经播放，只是改变当前播放速度为正常速度
*/
        [DllImport(@"HDVSDK_Play.dll")]
        public static extern bool HDVPLAY_Play(
            long lPlayHandle,
            IntPtr hPlayWnd,
             bool bCloseSound = false);


        /**
* 设置解码回调函数
* @param [IN] lPlayHandle 当前的解码句柄
* @param [IN] fDecodeFun  解码回调函数，可以为空(NULL)，NULL表示不再解码回调
* @param [IN] pUserData   用户自定义的数据，回调函数原值返回
* @return 返回如下结果：
* - 成功：true
* - 失败：false
* - 获取错误码调用HDVPLAY_GetLastError
* @note 该接口设置解码回调函数，用户可以自己处理解码后的媒体数据。注意解码部分不控制速度，
只要用户从回调函数中返回，解码器就会解码下一部分数据。
*/
        [DllImport(@"HDVSDK_Play.dll")]
        public static extern bool HDVPLAY_SetDecodeCallBack(
            Int32 lPlayHandle,
            CBDecodeFun fDecodeFun,
            IntPtr pUserData);



        /**
* @struct tagFrameInfo
* @brief 回调解码后的数据帧信息
* @attention 无
*/
        public struct tagFrameInfo
        {
            UInt32 nWidth;     /**< 画面宽，单位像素 */
            UInt32 nHeight;    /**< 画面高，单位像素 */
            UInt32 nStamp;     /**< 时标信息，单位毫秒 */
            tagFrametype eFrameType; //**< 数据类型，详见E_FRAME_TYPE定义说明 */
            UInt32 nFrameRate; /**< 视频帧率 */
        }




        /******************************************************************************
    SDKPLAY回调解码后的数据
    *******************************************************************************/
        /**
        * 解码回调函数指针类型
        * @param [IN] lPlayHandle 当前的解码句柄
        * @param [IN] pBuf        解码后的媒体数据
        * @param [IN] nBufSize    解码后的媒体数据pBuf的长度
        * @param [IN] pFrameInfo  解码后的媒体信息，详见S_FRAMEINFO定义
        * @param [IN] pUserData   用户数据，调用HDVPLAY_SetDecodeCallBack时用户输入的值
        * @return 无
        * @note 无
        */
        public delegate void CBDecodeFun(Int32 lPlayHandle,
        ref byte pBuf,
        UInt32 nBufSize,
        ref tagFrameInfo pFrameInfo,
        IntPtr pUserData);

        /**
* 输入媒体流数据，打开流之后才能输入数据
* @param [IN]   lPlayHandle     解码句柄，HDVPLAY_OpenStream的返回值
* @param [IN]   pStreamBuf      媒体流数据
* @param [IN]   lStreamBufSize  媒体流数据的大小，单位为字节
* @return 返回如下结果：
* - 成功：true
* - 失败：false
* - 获取错误码调用HDVPLAY_GetLastError
* @note 无
*/
        [DllImport(@"HDVSDK_Play.dll")]
        public static extern bool  HDVPLAY_InputData(Int32 lPlayHandle,
                                           IntPtr pStreamBuf,
                                          UInt32 lStreamBufSize);



                /**
        * 播放停止
        * @param [IN]   lPlayHandle 解码句柄
        * @return 返回如下结果：
        * - 成功：true
        * - 失败：false
        * - 获取错误码调用HDVPLAY_GetLastError
        * @note 无
        */
                [DllImport(@"HDVSDK_Play.dll")]
        public static extern bool HDVPLAY_Stop(long lPlayHandle);
        /**
        * 播放暂停
        * @param [IN]   lPlayHandle 解码句柄
        * @return 返回如下结果：
        * - 成功：true
        * - 失败：false
        * - 获取错误码调用HDVPLAY_GetLastError
        * @note 无
        */
        [DllImport(@"HDVSDK_Play.dll")]
        public static extern bool HDVPLAY_Pause(long lPlayHandle);
        /**
        * 播放恢复
        * @param [IN]   lPlayHandle 解码句柄
        * @return 返回如下结果：
        * - 成功：true
        * - 失败：false
        * - 获取错误码调用HDVPLAY_GetLastError
        * @note 无
        */
        [DllImport(@"HDVSDK_Play.dll")]
        public static extern bool HDVPLAY_Resume(long lPlayHandle);

       /**
        * 关闭流
        * @param [IN]   lPlayHandle 解码句柄，HDVPLAY_OpenStream的返回值
        * @return 返回如下结果：
        * - 成功：true
        * - 失败：false
        * - 获取错误码调用HDVPLAY_GetLastError
        * @note 无
        */
        [DllImport(@"HDVSDK_Play.dll")]
        public static extern bool HDVPLAY_CloseStream(long lPlayHandle);


    }
}
