using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System;
using System.IO;
using Android.App;
using System.Text;

namespace ToDoList_Ver2
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        Button btnDel;
        Button btnAdd;
        Button btnEdit;
        Button btnCreate;

        EditText enterText;
        TableLayout taskTable;

        List<Task> tasks;

        TableRow selectedRow;

        static string fileName = "tasks.txt";
        static string filePath;
        string oldFile;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            filePath = Path.Combine(FilesDir.Path, fileName);

            btnDel = FindViewById<Button>(Resource.Id.btnDel);
            btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
            btnEdit = FindViewById<Button>(Resource.Id.btnEdit);
            btnCreate = FindViewById<Button>(Resource.Id.btnCreate);

            enterText = FindViewById<EditText>(Resource.Id.editText1);
            taskTable = FindViewById<TableLayout>(Resource.Id.taskTable);

            TextView textViewDate = FindViewById<TextView>(Resource.Id.textView1);
            string currentDate = DateTime.Now.ToString("dd.MM");
            textViewDate.Text = $"To-Do List for {currentDate}";

            btnDel.Click += BtnDel_Click;
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnCreate.Click += BtnCreate_Click;

            if (File.Exists(filePath))
            {
                oldFile = ReadFromFile(filePath);
                tasks = ConvertFromString(oldFile);

                if (tasks != null)
                {
                    foreach (Task task in tasks)
                    {
                        AddTaskToTable(task);
                    }
                }
            }
            else
            {
                tasks = new List<Task>();
            }
        }

        private static List<Task> ConvertFromString(string oldFile)
        {
            List<Task> oldTasks = new List<Task>();
            var lines = oldFile.Split(new[] { System.Environment.NewLine }, StringSplitOptions.None);

            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 3)
                {
                    oldTasks.Add(new Task
                    {
                        Title = parts[0],
                        Description = parts[1],
                        IsCompleted = bool.Parse(parts[2])
                    });
                }
            }

            return oldTasks;
        }


        private void BtnAdd_Click(object sender, System.EventArgs e)
        {
            if (enterText.Visibility == ViewStates.Gone)
            {
                btnCreate.Visibility = ViewStates.Visible;
                enterText.Visibility = ViewStates.Visible;
            }
            else
            {
                enterText.Visibility = ViewStates.Gone;
            }
        }

        private void BtnCreate_Click(object sender, System.EventArgs e)
        {
            string taskText = enterText.Text;
            if (!string.IsNullOrEmpty(taskText))
            {
                Task task = new Task { Title = taskText, Description = " " };
                tasks.Add(task);
                AddTaskToTable(task);
                enterText.Text = string.Empty;
                Toast.MakeText(this, "Task is added", ToastLength.Short).Show();
            }

            enterText.Visibility = ViewStates.Gone;
            btnCreate.Visibility = ViewStates.Gone;
            
        }

        private void AddTaskToTable(Task task)
        {
            TableRow row = new TableRow(this);

            TextView taskTextView = new TextView(this)
            {
                Text = task.Title,
                TextSize = 20,
                Typeface = Android.Graphics.Typeface.Monospace,
                Background = new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.ParseColor("#F2BBBB"))
            };

            CheckBox completionCheckBox = new CheckBox(this)
            {
                Checked = task.IsCompleted,
                Background = new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.ParseColor("#F2BBBB"))
            };
            completionCheckBox.Click += (s, e) =>
            {
                task.IsCompleted = ((CheckBox)s).Checked;
            };

            row.AddView(taskTextView);
            row.AddView(completionCheckBox);
            row.Click += (s, e) =>
            {
                if (selectedRow != null)
                {
                    selectedRow.SetBackgroundColor(Android.Graphics.Color.Transparent);
                }
                selectedRow = (TableRow)s;
                selectedRow.SetBackgroundColor(Android.Graphics.Color.Gray);
            };

            int taskIndex = tasks.IndexOf(task);
            row.Tag = taskIndex;

            taskTable.AddView(row);
        }


        private void BtnEdit_Click(object sender, System.EventArgs e)
        {
            if (selectedRow != null)
            {
                TextView taskTextView = (TextView)selectedRow.GetChildAt(0);
                enterText.Text = taskTextView.Text;
                enterText.Visibility = ViewStates.Visible;
                btnCreate.Visibility = ViewStates.Visible;

                int taskIndex = (int)selectedRow.Tag;
                Task taskToEdit = tasks[taskIndex];
                tasks.Remove(taskToEdit);

                taskTable.RemoveView(selectedRow);

                selectedRow = null;
            }
        }

        private void BtnDel_Click(object sender, System.EventArgs e)
        {
            if (selectedRow != null)
            {
                int taskIndex = (int)selectedRow.Tag;
                Task taskToRemove = tasks[taskIndex];

                tasks.Remove(taskToRemove);
                taskTable.RemoveView(selectedRow);

                selectedRow = null;
            }
        }

        private void WriteToFile(string filePath)
        {
            try
            {
                using (var writer = new StreamWriter(filePath, append: false)) // false для перезаписи файла
                {
                    foreach (var task in tasks)
                    {
                        writer.WriteLine($"{task.Title}|{task.Description}|{task.IsCompleted}");
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Error writing to file: " + ex.Message, ToastLength.Short).Show();
            }
        }

        private string ReadFromFile(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Error reading from file: " + ex.Message, ToastLength.Short).Show();
                return null;
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            WriteToFile(filePath);
        }
    }
}
