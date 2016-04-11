﻿using Cad2D.Pages;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Hikvision;
using MahApps.Metro.Controls;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using System.Windows.Data;
using System.Windows.Media;
using System.Text.RegularExpressions;

namespace Cad2D
{
    public partial class CanvasCad2D : UserControl
    {
        private void Ls_connection_OnReadedSuccessfully(object sender, EventArgs e)
        {
            readingPacketInfo p = (readingPacketInfo)sender;
            try
            {

                if (plcInformation.positionX.readingPacket != null && plcInformation.positionX.readingPacket.order == p.order)
                {
                    plcInformation.positionX.value = (ushort)p.value;
                   OnGUIActions(() => setPositionX());
                    plcInformation.positionX.readingPacket = null;
                    return;
                }
                if (plcInformation.positionY.readingPacket != null && plcInformation.positionY.readingPacket.order == p.order)
                {
                    plcInformation.positionY.value = (ushort)p.value;
                    OnGUIActions(() => setPositionY());
                    plcInformation.positionY.readingPacket = null;
                    return;
                }
                if (plcInformation.alert.readingPacket != null && plcInformation.alert.readingPacket.order == p.order)
                {
                    plcInformation.alert.value = (ushort)p.value;
                    OnGUIActions(() => setAlarm());
                    plcInformation.alert.readingPacket = null;
                    return;
                }
                if (plcInformation.manualOrAuto.readingPacket != null && plcInformation.manualOrAuto.readingPacket.order == p.order)
                {
                    plcInformation.manualOrAuto.value = (ushort)p.value;
                    OnGUIActions(() => setManualOrAuto());
                    plcInformation.manualOrAuto.readingPacket = null;
                    return;
                }
                if (plcInformation.machinPhase.readingPacket != null && plcInformation.machinPhase.readingPacket.order == p.order)
                {
                    plcInformation.machinPhase.value = (ushort)p.value;
                    OnGUIActions(() => setMachinPhase());
                    plcInformation.machinPhase.readingPacket = null;
                    return;
                }
                ///
                if(plcUtilitisAndOptions.DiskOutOfStnFstAXY.readingPacket != null && plcUtilitisAndOptions.DiskOutOfStnFstAXY.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageToolsObject.textBox_TopValue.Value = p.value);
                    plcUtilitisAndOptions.DiskOutOfStnFstAXY.readingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.DiskOutOfStnFstAXX.readingPacket != null && plcUtilitisAndOptions.DiskOutOfStnFstAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageToolsObject.textBox_LeftValue.Value = p.value);
                    plcUtilitisAndOptions.DiskOutOfStnFstAXX.readingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.DiskOutOfStnEndAXY.readingPacket != null && plcUtilitisAndOptions.DiskOutOfStnEndAXY.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageToolsObject.textBox_BottomValue.Value = p.value);
                    plcUtilitisAndOptions.DiskOutOfStnEndAXY.readingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.DiskOutOfStnEndAXX.readingPacket != null && plcUtilitisAndOptions.DiskOutOfStnEndAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageToolsObject.textBox_RightValue.Value = p.value);
                    plcUtilitisAndOptions.DiskOutOfStnEndAXX.readingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.ClampAmount.readingPacket != null && plcUtilitisAndOptions.ClampAmount.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageToolsObject.textBox_ClampAmount.Value = p.value);
                    plcUtilitisAndOptions.ClampAmount.readingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.HashyeFront.readingPacket != null && plcUtilitisAndOptions.HashyeFront.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageToolsObject.textBox_ClampTopLeft.Value = p.value);
                    plcUtilitisAndOptions.HashyeFront.readingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.HashyeBack.readingPacket != null && plcUtilitisAndOptions.HashyeBack.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageToolsObject.textBox_ClampTopRight.Value = p.value);
                    plcUtilitisAndOptions.HashyeBack.readingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.HashyeEdge.readingPacket != null && plcUtilitisAndOptions.HashyeEdge.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageToolsObject.textBox_ClampBottomRight.Value = p.value);
                    plcUtilitisAndOptions.HashyeEdge.readingPacket = null;
                    return;
                }
                //////
                if (hscXHelper.testModeAXX.readingPacket != null && hscXHelper.testModeAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageHscX.checkBox.IsChecked = (p.value == 1) ? true : false);
                    hscXHelper.testModeAXX.readingPacket = null;
                    return;
                }
                if (hscXHelper.curPosMMAXX.readingPacket != null && hscXHelper.curPosMMAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageHscX.textBoxCurPosmm.Text = p.value.ToString());
                    hscXHelper.curPosMMAXX.readingPacket = null;
                    return;
                }
                if (hscXHelper.spdReqAXX.readingPacket != null && hscXHelper.spdReqAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageHscX.textBoxSpdReq.Text = p.value.ToString());
                    hscXHelper.spdReqAXX.readingPacket = null;
                    return;
                }
                if (hscXHelper.accAXX.readingPacket != null && hscXHelper.accAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageHscX.textBoxAcc.Text = p.value.ToString());
                    hscXHelper.accAXX.readingPacket = null;
                    return;
                }
                if (hscXHelper.decModeAXX.readingPacket != null && hscXHelper.decModeAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageHscX.textBoxDec.Text = p.value.ToString());
                    hscXHelper.decModeAXX.readingPacket = null;
                    return;
                }
                if (hscXHelper.manMaxTarPosAXX.readingPacket != null && hscXHelper.manMaxTarPosAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageHscX.textBoxManMax.Text = p.value.ToString());
                    hscXHelper.manMaxTarPosAXX.readingPacket = null;
                    return;
                }
                if (hscXHelper.manMinTarPosAXX.readingPacket != null && hscXHelper.manMinTarPosAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageHscX.textBoxManMin.Text = p.value.ToString());
                    hscXHelper.manMinTarPosAXX.readingPacket = null;
                    return;
                }
                if (hscXHelper.bwdKeshAXX.readingPacket != null && hscXHelper.bwdKeshAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageHscX.textBox_Copy86.Text = p.value.ToString());
                    hscXHelper.bwdKeshAXX.readingPacket = null;
                    return;
                }
                if (hscXHelper.fwdKeshAXX.readingPacket != null && hscXHelper.fwdKeshAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageHscX.textBoxFwdKesh.Text = p.value.ToString());
                    hscXHelper.fwdKeshAXX.readingPacket = null;
                    return;
                }
                if (hscXHelper.bwdMaxDispAXX.readingPacket != null && hscXHelper.bwdMaxDispAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageHscX.textBox_Copy87.Text = p.value.ToString());
                    hscXHelper.bwdMaxDispAXX.readingPacket = null;
                    return;
                }
                if (hscXHelper.fwdMaxDispAXX.readingPacket != null && hscXHelper.fwdMaxDispAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageHscX.textBoxMaxFwd.Text = p.value.ToString());
                    hscXHelper.fwdMaxDispAXX.readingPacket = null;
                    return;
                }
                if (hscXHelper.jogCalcAutoAXX.readingPacket != null && hscXHelper.jogCalcAutoAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageHscX.checkBox_Copy.IsChecked = (p.value == 1) ? true : false);
                    hscXHelper.jogCalcAutoAXX.readingPacket = null;
                    return;
                }
                if (hscXHelper.decAutoAXX.readingPacket != null && hscXHelper.decAutoAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageHscX.checkBox_Copy1.IsChecked = (p.value == 1) ? true : false);
                    hscXHelper.decAutoAXX.readingPacket = null;
                    return;
                }
                if (hscXHelper.decStopAXX.readingPacket != null && hscXHelper.decStopAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageHscX.textBoxDecStop.Text = p.value.ToString());
                    hscXHelper.decStopAXX.readingPacket = null;
                    return;
                }

                ////////////////////
                if (plcUtilitisAndOptions.DiskDiameter.readingPacket != null && plcUtilitisAndOptions.DiskDiameter.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageSettingObject.text_Disk_Diameter.Text = p.value.ToString());
                    plcUtilitisAndOptions.DiskDiameter.readingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.ParkPosAXX.readingPacket != null && plcUtilitisAndOptions.ParkPosAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageSettingObject.text_Park_Pos_AXX.Text = p.value.ToString());
                    plcUtilitisAndOptions.ParkPosAXX.readingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.ParkPosAXY.readingPacket != null && plcUtilitisAndOptions.ParkPosAXY.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageSettingObject.text_Park_Pos_AXY.Text = p.value.ToString());
                    plcUtilitisAndOptions.ParkPosAXY.readingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.MinDif.readingPacket != null && plcUtilitisAndOptions.MinDif.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageSettingObject.text_Min_Dif.Text = p.value.ToString());
                    plcUtilitisAndOptions.MinDif.readingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.AXXFeedDist.readingPacket != null && plcUtilitisAndOptions.AXXFeedDist.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageSettingObject.text_AXX_Feed_Dist.Text = p.value.ToString());
                    plcUtilitisAndOptions.AXXFeedDist.readingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.AXYFeedDist.readingPacket != null && plcUtilitisAndOptions.AXYFeedDist.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageSettingObject.text_AXY_Feed_Dist.Text = p.value.ToString());
                    plcUtilitisAndOptions.AXYFeedDist.readingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.Hashye.readingPacket != null && plcUtilitisAndOptions.Hashye.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageSettingObject.text_Hashye.Text = p.value.ToString());
                    plcUtilitisAndOptions.Hashye.readingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.ManSpdAXX.readingPacket != null && plcUtilitisAndOptions.ManSpdAXX.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageSettingObject.text_Man_Spd_AXX.Text = p.value.ToString());
                    plcUtilitisAndOptions.ManSpdAXX.readingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.ManSpdAXY.readingPacket != null && plcUtilitisAndOptions.ManSpdAXY.readingPacket.order == p.order)
                {
                    OnGUIActions(() => pageSettingObject.text_Man_Spd_AXY.Text = p.value.ToString());
                    plcUtilitisAndOptions.ManSpdAXY.readingPacket = null;
                    return;
                }
                if (diskDiameter.readingPacket != null && diskDiameter.readingPacket.order == p.order)
                {
                    OnGUIActions(() => setDiskDiameter((ushort)p.value));
                    diskDiameter.readingPacket = null;
                    return;
                }
                if (plcInformation.water.readingPacket != null && plcInformation.water.readingPacket.order == p.order)
                {
                    plcInformation.water.value = (ushort)p.value;
                    OnGUIActions(() => setWaterSetting());
                    plcInformation.water.readingPacket = null;
                    return;
                }
                if (plcInformation.waterTOff.readingPacket != null && plcInformation.waterTOff.readingPacket.order == p.order)
                {
                    plcInformation.waterTOff.value = (ushort)p.value;
                    OnGUIActions(() => textBox_Timer_Off.Text = plcInformation.waterTOff.value.ToString());
                    plcInformation.waterTOff.readingPacket = null;
                    return;
                }
                if (plcInformation.waterTOn.readingPacket != null && plcInformation.waterTOn.readingPacket.order == p.order)
                {
                    plcInformation.waterTOn.value = (ushort)p.value;
                    OnGUIActions(() => textBox_Timer_On.Text = plcInformation.waterTOn.value.ToString());
                    plcInformation.waterTOn.readingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.Encoder.EncoderXPals.readingPacket != null && plcUtilitisAndOptions.Encoder.EncoderXPals.readingPacket.order == p.order)
                {
                    plcUtilitisAndOptions.Encoder.EncoderXPals.value = (ushort)p.value;
                    plcUtilitisAndOptions.Encoder.EncoderXPals.readingPacket = null;
                    if (pageSettingObject != null)
                        pageSettingObject.readPosX();
                    return;
                }
                if (plcUtilitisAndOptions.Encoder.EncoderXPos.readingPacket != null && plcUtilitisAndOptions.Encoder.EncoderXPos.readingPacket.order == p.order)
                {
                    plcUtilitisAndOptions.Encoder.EncoderXPos.value = (ushort)p.value;
                    plcUtilitisAndOptions.Encoder.EncoderXPos.readingPacket = null;
                    if (pageSettingObject != null)
                        pageSettingObject.readPalsY();
                    return;
                }
                if (plcUtilitisAndOptions.Encoder.EncoderYPals.readingPacket != null && plcUtilitisAndOptions.Encoder.EncoderYPals.readingPacket.order == p.order)
                {
                    plcUtilitisAndOptions.Encoder.EncoderYPals.value = (ushort)p.value;
                    plcUtilitisAndOptions.Encoder.EncoderYPals.readingPacket = null;
                    if (pageSettingObject != null)
                        pageSettingObject.readPosY();
                    return;
                }
                if (plcUtilitisAndOptions.Encoder.EncoderYPos.readingPacket != null && plcUtilitisAndOptions.Encoder.EncoderYPos.readingPacket.order == p.order)
                {
                    plcUtilitisAndOptions.Encoder.EncoderYPos.value = (ushort)p.value;
                    plcUtilitisAndOptions.Encoder.EncoderYPos.readingPacket = null;
                    if (pageSettingObject != null)
                        pageSettingObject.OnGUIActions(() => pageSettingObject.setNewChanges());
                    return;
                }

            }
            catch (Exception ex)
            {
                Logger.LogError("_Message : " + ex.Message + "\n\n_Source : " + ex.Source + "\n\n_TargetSite : " + ex.TargetSite + "\n\n _ALL : " + ex.ToString(), LogType.Error,ex);
            }
        }


        private void Ls_connection_OnWritedSuccessfully(object sender, EventArgs e)
        {
            writingPacketInfo p = (writingPacketInfo)sender;
            try
            {
                
                if (plcInformation.water.writingPacket != null && plcInformation.water.writingPacket.order == p.order)
                {
                    plcInformation.water.writingPacket = null;
                       OnGUIActions(() => setWaterSetting());
                    return;
                }
                ///////////// 
                if (directionTypePacket != null && directionTypePacket.writingPacket != null && directionTypePacket.writingPacket.order == p.order)
                {
                    directionTypePacket.writingPacket = null;
                    if (sendingInFirstTime)
                        _sendDataToPlc();
                    return;
                }
                //////////////////encoder /////////////////////
                if (plcUtilitisAndOptions.Encoder.EncoderXMult.writingPacket != null && plcUtilitisAndOptions.Encoder.EncoderXMult.writingPacket.order == p.order)
                {
                    plcUtilitisAndOptions.Encoder.EncoderXMult.writingPacket = null;
                    if (pageSettingObject != null)
                        pageSettingObject.OnGUIActions(() => pageSettingObject.sendingDivXValueToPlc());
                    return;
                }

                if (plcUtilitisAndOptions.Encoder.EncoderYMult.writingPacket != null && plcUtilitisAndOptions.Encoder.EncoderYMult.writingPacket.order == p.order)
                {
                    plcUtilitisAndOptions.Encoder.EncoderYMult.writingPacket = null;
                    if (pageSettingObject != null)
                        pageSettingObject.OnGUIActions(() => pageSettingObject.sendingDivYValueToPlc());
                    return;
                }
                if (plcUtilitisAndOptions.Encoder.EncoderXDiv.writingPacket != null && plcUtilitisAndOptions.Encoder.EncoderXDiv.writingPacket.order == p.order)
                {
                    plcUtilitisAndOptions.Encoder.EncoderXDiv.writingPacket = null;
                    return;
                }
                if (plcUtilitisAndOptions.Encoder.EncoderYDiv.writingPacket != null && plcUtilitisAndOptions.Encoder.EncoderYDiv.writingPacket.order == p.order)
                {
                    plcUtilitisAndOptions.Encoder.EncoderYDiv.writingPacket = null;
                    return;
                }
               
                if (shutDownPacketId != null && shutDownPacketId.order == p.order)
                {
                    shutDownPacketId = null;
                    shoutDownThePanelPC(4);
                    return;
                }
                if (positionxPacketInfo != null && positionxPacketInfo.order == p.order)
                {
                    is_inSendingx = false;
                    return;
                }
                if (positionyPacketInfo != null && positionyPacketInfo.order == p.order)
                {
                    is_inSendingy = false;
                    return;
                }

                foreach (writingPacketInfo packet in writingPackets)
                {
                    if (p.order == packet.order)
                    {
                        writingPackets.Remove(packet);
                        if (stoneScanPacketCounter < stoneScanPacketCount)
                        {
                            stoneScanPacketCounter++;
                            break;
                        }
                        else
                        {
                            if (horizonalBoundryCounter < horizonalBoundryCount)
                            {
                                horizonalBoundryCounter++;
                                break;
                            }
                            else
                            {
                                if (verticalBoundryCounter < verticalBoundryCount)
                                {
                                    verticalBoundryCounter++;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (stoneScanPacketCounter < stoneScanPacketCount)
                {

                    if (lsConnection.Connected)
                        lsConnection.writeToPlc(DataType.WORD, stoneScan[stoneScanPacketCounter], scanAriaSegment + stoneScanPacketCounter, ref writingPackets);

                }
                else
                {
                    if (horizonalBoundryCounter < horizonalBoundryCount)
                    {

                        lsConnection.writeToPlc(DataType.WORD, stoneHorizontalEdge[horizonalBoundryCounter], horizonalBoundrySegment + horizonalBoundryCounter, ref writingPackets);

                    }
                    else
                    {
                        if (verticalBoundryCounter < verticalBoundryCount)
                        {

                            if (lsConnection.Connected)
                                lsConnection.writeToPlc(DataType.WORD, stoneVerticalEdge[verticalBoundryCounter], verticalBoundrySegment + verticalBoundryCounter, ref writingPackets);

                        }
                        else
                        {
                            if (pagesStack.Count == 0)
                            {
                                OnGUIActions(() => writeToPlcFinished());
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("_Message : " + ex.Message + "\n\n_Source : " + ex.Source + "\n\n_TargetSite : " + ex.TargetSite + "\n\n _ALL : " + ex.ToString(), LogType.Error ,ex);
            }
        }
        private void Ls_connection_OnReadedContinuous(object sender, EventArgs e)
        {
            readingPacketCountinus repi = (readingPacketCountinus)sender;
            return;

            //int i = plcUtilitisAndOptions.Encoder.PackestIdX.FindIndex(x => x == repi.order);
            //if (i >= 0)
            //{
            //    plcUtilitisAndOptions.Encoder.PackestIdX.RemoveAt(i);
            //    plcUtilitisAndOptions.Encoder.updateEncoderXValues(repi.continuousData);
            //    if (pageSettingObject == null) return;
            //    pageSettingObject.readEncoderYValues();
            //    return;
            //}

            //i = plcUtilitisAndOptions.Encoder.PackestIdY.FindIndex(x => x == repi.order);
            //if (i >= 0)
            //{
            //    plcUtilitisAndOptions.Encoder.PackestIdY.RemoveAt(i);
            //    plcUtilitisAndOptions.Encoder.updateEncoderYValues(repi.continuousData);
            //    if (pageSettingObject == null) return;
            //    return;
            //}

            //i = plcUtilitisAndOptions.Velocity.PackestId.FindIndex(x => x == repi.order);
            //if (i >= 0)
            //{
            //    plcUtilitisAndOptions.Velocity.PackestId.RemoveAt(i);
            //    plcUtilitisAndOptions.Velocity.updateValues(repi.continuousData);
            //    OnGUIActions(() => setSlidersValues());
            //    //////////
            //    return;
            //}
        }

        void setAlarm()
        {
            bool[] array = Convert.ToString(plcInformation.alert.value, 2 /*for binary*/).Select(s => s.Equals('1')).ToArray();
            bool[] boolArray = createBoolArray(array);

            for (int i = 0; i < 16; i++)
            {
                switch (i)
                {
                    case 0:
                        if (boolArray[i]) { alarm |= 1; } else { alarm &= 65534; }
                        break;
                    case 1:
                        if (boolArray[i]) { alarm |= 2; } else { alarm &= 65533; }
                        break;
                    case 2:
                        if (boolArray[i]) { alarm |= 4; } else { alarm &= 65531; }
                        break;
                    case 3:
                        if (boolArray[i]) { alarm |= 8; } else { alarm &= 65527; }
                        break;
                    case 4:
                        if (boolArray[i]) { alarm |= 16; } else { alarm &= 65519; }
                        break;
                    case 5:
                        if (boolArray[i]) { alarm |= 32; } else { alarm &= 65503; }
                        break;
                    case 6:
                        if (boolArray[i]) { alarm |= 64; } else { alarm &= 65471; }
                        break;
                    case 7:
                        if (boolArray[i]) { alarm |= 128; } else { alarm &= 65407; }
                        break;
                    case 11:
                        if (boolArray[i]) { alarm |= 2048; } else { alarm &= 63487; }
                        break;
                    case 10:
                        if (boolArray[i]) { alarm |= 1024; } else { alarm &= 64511; }
                        break;
                    default:
                        break;
                }
            }
        }

        void setMachinPhase()
        {
            if (plcInformation.machinPhase.value == 1)
            {
                label_Phase.Content = "READY";
                label_Phase.Foreground = Brushes.Green;
                return;
            }
            else if (plcInformation.machinPhase.value == 2)
            {
                label_Phase.Content = "RUN";
                label_Phase.Foreground = Brushes.DodgerBlue;
                return;
            }
            else if (plcInformation.machinPhase.value == 4)
            {
                label_Phase.Content = "PARK";
                label_Phase.Foreground = Brushes.OrangeRed;
                return;
            }
            else if (plcInformation.machinPhase.value == 8)
            {
                label_Phase.Content = "HOLD";
                label_Phase.Foreground = Brushes.Yellow;
                return;
            }
            else
            {
                label_Phase.Content = "STOP";
                label_Phase.Foreground = Brushes.Black;
                return;
            }
        }
        void setManualOrAuto()
        {
            if (plcInformation.manualOrAuto.value == 1)
            {
                image_AutoOrManual.Source = new BitmapImage(new Uri("pack://application:,,,/Cad2D;component/Resources/power.png"));
                return;
            }
            else if (plcInformation.manualOrAuto.value == 2)
            {
                image_AutoOrManual.Source = new BitmapImage(new Uri("pack://application:,,,/Cad2D;component/Resources/manual.png"));
                return;
            }
            else if (plcInformation.manualOrAuto.value == 4)
            {
                image_AutoOrManual.Source = new BitmapImage(new Uri("pack://application:,,,/Cad2D;component/Resources/auto.png"));
                return;
            }
            else
            {
                image_AutoOrManual.Source = new BitmapImage(new Uri("pack://application:,,,/Cad2D;component/Resources/power.png"));
                return;
            }
        }

        void setPositionX()
        {
            labelPosX.Content = plcInformation.positionX.value;
            double zaribx = (maxHorizontalSlice - minHorizontalSlice) / (endPoint.X - startPoint.X);
            double xPos = (plcInformation.positionX.value / zaribx) + startPoint.X - (minHorizontalSlice / zaribx);

            Canvas.SetLeft(mainCanvas.Children[headPosition], xPos);
        }
        void setPositionY()
        {
            labelPosY.Content = plcInformation.positionY.value;
            double zaribY = (maxVerticalSlice - minVerticalSlice) / (endPoint.Y - startPoint.Y);
            double yPos = (plcInformation.positionY.value / zaribY) + startPoint.Y - (minVerticalSlice / zaribY);

            Canvas.SetTop(mainCanvas.Children[headPosition], yPos);
        }
        private void setWaterSetting()
        {
            bool[] array = Convert.ToString(plcInformation.water.value, 2 /*for binary*/).Select(s => s.Equals('1')).ToArray();
            bool[] boolArray = createBoolArray(array);
            if (boolArray[0])
                setWaterOn();
            else
            {
                setWaterOff();
                return;
            }
            if (boolArray[1])
                setWaterTimerOn();
            else
            {
                setWaterTimerOff();
                return;
            }
        }

        void setWaterOff()
        {
            textBox_Timer_Off.IsEnabled = false;
            textBox_Timer_On.IsEnabled = false;
            button_Water_Timer.IsEnabled = false;
            button_Water_Timer.Opacity = 65;
            img_water_timer.Opacity = 65;
            img_water.Source= new BitmapImage(new Uri("pack://application:,,,/Cad2D;component/Resources/WaterOption/NotWater.png"));
        }
        void setWaterOn()
        {
            textBox_Timer_Off.IsEnabled = true;
            textBox_Timer_On.IsEnabled = true;
            button_Water_Timer.IsEnabled = true;
            button_Water_Timer.Opacity = 100;
            img_water_timer.Opacity = 100;
            img_water.Source = new BitmapImage(new Uri("pack://application:,,,/Cad2D;component/Resources/WaterOption/Water.png"));
        }
        void setWaterTimerOn()
        {
            textBox_Timer_Off.IsEnabled = true;
            textBox_Timer_On.IsEnabled = true;
            img_water_timer.Source = new BitmapImage(new Uri("pack://application:,,,/Cad2D;component/Resources/WaterOption/Timer.png"));
        }
        void setWaterTimerOff()
        {
            textBox_Timer_Off.IsEnabled = false;
            textBox_Timer_On.IsEnabled = false;
            img_water_timer.Source = new BitmapImage(new Uri("pack://application:,,,/Cad2D;component/Resources/WaterOption/NotTimer.png"));
        }
    }
}
