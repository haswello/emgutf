﻿//----------------------------------------------------------------------------
//  Copyright (C) 2004-2017 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Emgu.TF;
using System.IO;

namespace Emgu.TF.Models
{
    public class Inception : DownloadableModels
    {
        public Inception(Status status = null, String[] modelFiles = null, String downloadUrl = null)
            : base(
                modelFiles ?? new string[] { "tensorflow_inception_graph.pb", "imagenet_comp_graph_label_strings.txt" },
                downloadUrl ?? "https://github.com/emgucv/models/raw/master/inception/")
        {
            Download();

#if __ANDROID__
            byte[] model = File.ReadAllBytes(System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads, _modelFiles[0]));
#else
            byte[] model = File.ReadAllBytes(_modelFiles[0]);
#endif

            Buffer modelBuffer = Buffer.FromString(model);

            using (ImportGraphDefOptions options = new ImportGraphDefOptions())
                ImportGraphDef(modelBuffer, options, status);
        }

        public String[] Labels
        {
            get
            {
#if __ANDROID__
                return File.ReadAllLines(System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                    Android.OS.Environment.DirectoryDownloads, _modelFiles[1]));
#else
                return File.ReadAllLines(_modelFiles[1]);
#endif
            }
        }

        public float[] Recognize(Tensor image)
        {
            Session inceptionSession = new Session(this);
            Tensor[] finalTensor = inceptionSession.Run(new Output[] { this["input"] }, new Tensor[] { image },
                new Output[] { this["output"] });
            float[] probability = finalTensor[0].GetData(false) as float[];
            return probability;
        }
    }
}
