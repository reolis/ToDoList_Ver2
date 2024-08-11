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
using Android.Support.V4.Content;
using Android;
using Android.Content.PM;
using Android.Support.V4.App;
using Xamarin.Essentials;
using Android.Graphics;

namespace ToDoList_Ver2
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        Button btnDel;
        Button btnAdd;
        Button btnEdit;
        Button btnCreate;
        ImageButton btnSelectImage;

        EditText enterText;
        TableLayout taskTable;

        List<Task> tasks;

        TableRow selectedRow;

        ImageView imageFromUser;

        static string fileName = "tasks.txt";
        static string filePath;
        string oldFile;

        const int RequestStorageId = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            filePath = System.IO.Path.Combine(FilesDir.Path, fileName);

            btnDel = FindViewById<Button>(Resource.Id.btnDel);
            btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
            btnEdit = FindViewById<Button>(Resource.Id.btnEdit);
            btnCreate = FindViewById<Button>(Resource.Id.btnCreate);
            enterText = FindViewById<EditText>(Resource.Id.editText1);
            taskTable = FindViewById<TableLayout>(Resource.Id.taskTable);
            imageFromUser = FindViewById<ImageView>(Resource.Id.imageFromUser);
            btnSelectImage = FindViewById<ImageButton>(Resource.Id.btnSelectImage);
            TextView textViewDate = FindViewById<TextView>(Resource.Id.textView1);
            string currentDate = DateTime.Now.ToString("dd.MM");
            textViewDate.Text = $"To-Do List for {currentDate}";

            btnDel.Click += BtnDel_Click;
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnCreate.Click += BtnCreate_Click;
            btnSelectImage.Click += BtnSelectImage_Click;

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


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestStorageId)
            {
                bool allPermissionsGranted = true;
                for (int i = 0; i < grantResults.Length; i++)
                {
                    if (grantResults[i] != Permission.Granted)
                    {
                        allPermissionsGranted = false;
                        break;
                    }
                }

                if (allPermissionsGranted)
                {
                    AccessExternalStorage();
                }
                else
                {
                    Toast.MakeText(this, "You can't add image", ToastLength.Short).Show();
                }
            }
        }

        private void AccessExternalStorage()
        {
            ChooseBackground();

            //string backgroundImagePath = Preferences.Get("BackgroundImagePath", null);

            //if (!string.IsNullOrEmpty(backgroundImagePath) && File.Exists(backgroundImagePath))
            //{
            //    Bitmap bitmap = BitmapFactory.DecodeFile(backgroundImagePath);
            //    imageFromUser.SetImageBitmap(bitmap);
            //}
            //else
            //{
            //    Toast.MakeText(this, "No background image found. Please select an image.", ToastLength.Short).Show();
            //}
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

        public async void ChooseBackground()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Please select an image",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    using (var stream = await result.OpenReadAsync())
                    {
                        if (stream != null)
                        {
                            var image = BitmapFactory.DecodeStream(stream);
                            if (image != null)
                            {
                                imageFromUser.SetImageBitmap(image);

                                var localPath = SaveImageLocally(result.FileName, image);
                                SaveBackgroundImagePath(localPath);
                            }
                            else
                            {
                                Toast.MakeText(this, "Failed to decode image.", ToastLength.Short).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, "Failed to open stream.", ToastLength.Short).Show();
                        }
                    }
                }
                else
                {
                    Toast.MakeText(this, "Image selection was cancelled.", ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, $"Error: {ex.Message}", ToastLength.Short).Show();
            }
        }



        private string SaveImageLocally(string fileName, Bitmap bitmap)
        {
            var localPath = System.IO.Path.Combine(Application.Context.GetExternalFilesDir(null).AbsolutePath, fileName);

            using (var fileStream = new FileStream(localPath, FileMode.Create))
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, fileStream);
            }

            if (!File.Exists(localPath))
            {
                throw new InvalidOperationException("Failed to save image.");
            }

            return localPath;
        }


        private void SaveBackgroundImagePath(string path)
        {
            Preferences.Set("BackgroundImagePath", path);
        }


        private void BtnSelectImage_Click(object sender, System.EventArgs e)
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != Permission.Granted ||
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[]
                {
                    Manifest.Permission.ReadExternalStorage,
                    Manifest.Permission.WriteExternalStorage
                }, RequestStorageId);
            }
            else
            {
                AccessExternalStorage();
            }
            
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
