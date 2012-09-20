/*
 * CLangVS - Compiler Bridge for CLang in MS Visual Studio
 * Harry Denholm, ishani.org 2011
 *
 * Released under LLVM Release License. See LICENSE.TXT for details.
 */


using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClangVSx
{
  public partial class CVXSettings : Form
  {
    /// <summary>
    /// Settings management
    /// </summary>
    public CVXSettings()
    {
      InitializeComponent();

      cvxLocation.Text = CVXRegistry.PathToClang;
      
      cvxShowCmds.Checked = CVXRegistry.ShowCommands;
      cvxBatch.Checked = CVXRegistry.MakeBatchFiles;
      cvxEcho.Checked = CVXRegistry.EchoInternal;
      cvxPhases.Checked = CVXRegistry.ShowPhases;

      cvxCommonArgs.Text = CVXRegistry.CommonArgs;
      cvxTripleWin32.Text = CVXRegistry.TripleWin32;
      cvxTripleX64.Text = CVXRegistry.TripleX64;
      cvxTripleARM.Text = CVXRegistry.TripleARM;

      //DevO: 20.09.2012
      cvxEnableCpp11.Checked = CVXRegistry.EnableCpp11;
      cvxEnableMsABI.Checked = CVXRegistry.EnableMsABI;

      // blot the version number up in the title bar
      Assembly assem = Assembly.GetExecutingAssembly();
      Version vers = assem.GetName().Version;
      this.Text = "ClangVSx " + vers.ToString();
    }

    private bool validEXELocation(String loc)
    {
      // scientific! -.-
      return (System.IO.File.Exists(loc) && loc.ToLower().EndsWith(".exe"));
    }

    private void cvxBrowse_Click(object sender, EventArgs e)
    {
      findClangExe.InitialDirectory = System.IO.Path.GetDirectoryName(cvxLocation.Text);
      if (findClangExe.ShowDialog(this) == DialogResult.OK)
      {
        cvxLocation.Text = findClangExe.FileName;
      }
    }

    private void cvxDone_Click(object sender, EventArgs e)
    {
      {
        // save back to the registry
        CVXRegistry.PathToClang.Value = cvxLocation.Text;

        CVXRegistry.ShowCommands.Value = cvxShowCmds.Checked;
        CVXRegistry.MakeBatchFiles.Value = cvxBatch.Checked;
        CVXRegistry.EchoInternal.Value = cvxEcho.Checked;
        CVXRegistry.ShowPhases.Value = cvxPhases.Checked;

        CVXRegistry.CommonArgs.Value = cvxCommonArgs.Text;
        CVXRegistry.TripleWin32.Value = cvxTripleWin32.Text;
        CVXRegistry.TripleX64.Value = cvxTripleX64.Text;
        CVXRegistry.TripleARM.Value = cvxTripleARM.Text;

        //DevO: 20.09.2012
        CVXRegistry.EnableCpp11.Value = cvxEnableCpp11.Checked;
        CVXRegistry.EnableMsABI.Value = cvxEnableMsABI.Checked;
      }

      if (!validEXELocation(cvxLocation.Text))
      {
        MessageBox.Show("Warning: Cannot find file specified for CLANG.EXE, will be unable to build projects.", "ClangVSx Settings");
      }

      DialogResult = DialogResult.OK;
      Close();
    }

    private void cvxLocation_TextChanged(object sender, EventArgs e)
    {
      if (validEXELocation(cvxLocation.Text))
      {
        String cvxStatsStr = "";

        // execute the compiler, ask for version info
        System.Diagnostics.Process compileProcess = new System.Diagnostics.Process();
        compileProcess.StartInfo.FileName = cvxLocation.Text;
        compileProcess.StartInfo.Arguments = "-v";
        compileProcess.StartInfo.UseShellExecute = false;
        compileProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        compileProcess.StartInfo.CreateNoWindow = true;
        compileProcess.StartInfo.RedirectStandardOutput = true;
        compileProcess.StartInfo.RedirectStandardError = true;
        compileProcess.Start();

        cvxStatsStr += compileProcess.StandardError.ReadLine() + "\r\n";
        cvxStatsStr += compileProcess.StandardError.ReadLine();

        compileProcess.StandardError.ReadToEnd();
        compileProcess.WaitForExit();

        // version text contains 'clang', we can assume that's a pass
        if (cvxStatsStr.Contains("clang"))
        {
          cvxStats.Text = cvxStatsStr;
          cvxPic.Visible = true;
        }
        else
        {
          cvxStats.Text = "Cannot validate file to be Clang Compiler";
          cvxPic.Visible = false;
        }
      }
      else
      {
        cvxStats.Text = "Cannot find file specified";
        cvxPic.Visible = false;
      }
    }

    private void url_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
    }

    private void cvxCancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

  }
}