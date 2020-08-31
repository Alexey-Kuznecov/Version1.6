$Form1 = New-Object -TypeName System.Windows.Forms.Form
[System.Windows.Forms.CheckBox]$CheckBox1 = $null
function InitializeComponent
{
$CheckBox1 = (New-Object -TypeName System.Windows.Forms.CheckBox)
$Form1.SuspendLayout()
#
#CheckBox1
#
$CheckBox1.Location = (New-Object -TypeName System.Drawing.Point -ArgumentList @([System.Int32]35,[System.Int32]22))
$CheckBox1.Name = [System.String]'CheckBox1'
$CheckBox1.Size = (New-Object -TypeName System.Drawing.Size -ArgumentList @([System.Int32]104,[System.Int32]24))
$CheckBox1.TabIndex = [System.Int32]0
$CheckBox1.Text = [System.String]'CheckBox1'
$CheckBox1.UseCompatibleTextRendering = $true
$CheckBox1.UseVisualStyleBackColor = $true
#
#Form1
#
$Form1.Controls.Add($CheckBox1)
$Form1.Text = [System.String]'Form1'
$Form1.ResumeLayout($false)
Add-Member -InputObject $Form1 -Name base -Value $base -MemberType NoteProperty
Add-Member -InputObject $Form1 -Name CheckBox1 -Value $CheckBox1 -MemberType NoteProperty
}
. InitializeComponent
