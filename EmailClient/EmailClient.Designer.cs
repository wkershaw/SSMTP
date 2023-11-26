namespace EmailClient
{
	partial class EmailClient
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			SendButton = new Button();
			AttachmentDialog = new OpenFileDialog();
			AddAttachmentButton = new Button();
			ContentTextBox = new RichTextBox();
			StatusMessage = new Label();
			SuspendLayout();
			// 
			// SendButton
			// 
			SendButton.Location = new Point(375, 563);
			SendButton.Name = "SendButton";
			SendButton.Size = new Size(75, 23);
			SendButton.TabIndex = 0;
			SendButton.Text = "Send";
			SendButton.UseVisualStyleBackColor = true;
			SendButton.Click += SendButton_Click;
			// 
			// AttachmentDialog
			// 
			AttachmentDialog.FileName = "Attachment";
			AttachmentDialog.Title = "Add Email Attachment";
			// 
			// AddAttachmentButton
			// 
			AddAttachmentButton.Location = new Point(294, 563);
			AddAttachmentButton.Name = "AddAttachmentButton";
			AddAttachmentButton.Size = new Size(75, 23);
			AddAttachmentButton.TabIndex = 2;
			AddAttachmentButton.Text = "Add Attachment";
			AddAttachmentButton.UseVisualStyleBackColor = true;
			// 
			// ContentTextBox
			// 
			ContentTextBox.Location = new Point(12, 12);
			ContentTextBox.Name = "ContentTextBox";
			ContentTextBox.Size = new Size(438, 545);
			ContentTextBox.TabIndex = 3;
			ContentTextBox.Text = "";
			// 
			// ErrorText
			// 
			StatusMessage.Location = new Point(12, 567);
			StatusMessage.Name = "ErrorText";
			StatusMessage.Size = new Size(276, 19);
			StatusMessage.TabIndex = 4;
			// 
			// EmailClient
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackgroundImageLayout = ImageLayout.Stretch;
			ClientSize = new Size(462, 598);
			Controls.Add(StatusMessage);
			Controls.Add(ContentTextBox);
			Controls.Add(AddAttachmentButton);
			Controls.Add(SendButton);
			Name = "EmailClient";
			Text = "Email Test Client";
			ResumeLayout(false);
		}

		#endregion

		private Button SendButton;
		private OpenFileDialog AttachmentDialog;
		private Button AddAttachmentButton;
		private RichTextBox ContentTextBox;
		private Label StatusMessage;
	}
}
