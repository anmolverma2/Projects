
const sql = require('mssql');
const conn = require('../DB/conn');
const dal = require('../DAL');
const { Salesmen } = require('../models/SalesmenModel');
const multer = require('multer');
const fs = require('fs');
const path = require('path');



var SalesmenKYC = async (req,res) =>{
    try {
       
        const UUID = req.query.UUID ; 
        const salesmen = new Salesmen(UUID);
        if (!UUID) {
          return res.status(400)
          .render("../views/shared/_LayoutResponsive", { errorMessage: "UUID parameter is required",pageContent: "error404" });
        }
        
        var data = await salesmen.DSRDetails(UUID);
        var user = await salesmen.ProfileDetails(UUID);

        if (!data) {
          return res.status(404)
          .render("../views/shared/_LayoutResponsive", { errorMessage: "DSR details not found",pageContent: "error404" });
        }
        
        res.render("../views/shared/_LayoutResponsive", {
          pageContent: "../Parle/DSR/dsrkyc",
          data: data,
          user: user,
          UUID: UUID,
        });
      
      
      } catch (err) {
        console.error("Error:", err);
        res.status(500)
        .render("../views/shared/_LayoutResponsive", { errorMessage: "Internal Server Error",pageContent: "error404" });
      }

}

var UploadDocument = async (req,res) =>{
    try {
        const UUID = req.query.UUID ; 
        
        if (!UUID) {
          return res.status(400)
          .render("../views/shared/_Document", { errorMessage: "UUID parameter is required",pageContent: "error404" });
        }
        
        res.render("../views/shared/_Document", {
          pageContent: "../Parle/DSR/upload-document",    
          UUID: UUID,
        });
      } catch (err) {
        console.error("Error:", err);
        res.status(500)
        .render("../views/shared/_Document", { errorMessage: "Internal Server Error",pageContent: "error404" });
      }

}

var ParentID = async (req,res) =>{
  try {
      const UUID = req.query.UUID ; 
      
      const salesmen = new Salesmen(UUID);

      if (!UUID) {
        return res.status(400)
        .render("../views/shared/_Document", { errorMessage: "UUID parameter is required",pageContent: "error404" });
      }
      
      var result = await salesmen.GetParentUUID(UUID[0]);

      var parentUUID = result[0];
      res.status(200).json({ parentUUID: parentUUID });

      
    } catch (err) {
      console.error("Error:", err);
      res.status(500)
      .render("../views/shared/_Document", { errorMessage: "Internal Server Error",pageContent: "error404" });
    }

}

// var PersonalDetail = async (req, res) => {
//   try {
//     uploadImage(req, res, async function (err) {
//       if (err) {
//         console.error("Error:", err);
//         return res.redirect("/UploadDocument");
//       } else {
//         const UUID = req.query.UUID;
//         const { productimage1, productimage2, productimage3, productimage5 } = req.files;
//         console.log(productimage1, productimage2, productimage3, productimage5);
//         const salesmen = new Salesmen(UUID);

//         if (!UUID) {
//           return res.status(400).render("../views/shared/_Document", { errorMessage: "UUID parameter is required", pageContent: "error404" });
//         }

//         var data = await salesmen.DSRDetails(UUID);

//         if (!data) {
//           return res.status(404).render("../views/shared/_Document", { errorMessage: "DSR details not found", pageContent: "error404" });
//         }

//         res.render("../views/shared/_Document", {
//           pageContent: "../Parle/DSR/personal-detail",
//           data: data,
//           UUID: UUID
//         });
//       }
//     });
//   } catch (err) {
//     console.error("Error:", err);
//     res.status(500).render("../views/shared/_Document", { errorMessage: "Internal Server Error", pageContent: "error404" });
//   }
// }


var PersonalDetail = async (req, res) => {
  try {
    uploadImage(req, res, async function (err) {
      if (err) {
        console.error("Error:", err);
        return res.redirect("/UploadDocument");
      } else {
        const UUID = req.query.UUID;
        const { productimage1, productimage2, productimage3, productimage5 } = req.files;
        
        const salesmen = new Salesmen(UUID);

        if (!UUID) {
          return res.status(400).render("../views/shared/_Document", { errorMessage: "UUID parameter is required", pageContent: "error404" });
        }

        var data = await salesmen.DSRDetails(UUID);

        if (!data) {
          return res.status(404).render("../views/shared/_Document", { errorMessage: "DSR details not found", pageContent: "error404" });
        }

        // Assuming the uploaded images are stored in the 'uploads/images/' directory
        const productImagePath1 = productimage1 ? '/uploads/images/' + productimage1[0].filename : '';
        const productImagePath2 = productimage2 ? '/uploads/images/' + productimage2[0].filename : '';
        const productImagePath3 = productimage3 ? '/uploads/images/' + productimage3[0].filename : '';
        const productImagePath5 = productimage5 ? '/uploads/images/' + productimage5[0].filename : '';

        // Here you can save the image paths in variables or process them further
        
        res.render("../views/shared/_Document", {
          pageContent: "../Parle/DSR/personal-detail",
          data: data,
          UUID: UUID,
          productImagePath1: productImagePath1,
          productImagePath2: productImagePath2,
          productImagePath3: productImagePath3,
          productImagePath5: productImagePath5
        });
      }
    });
  } catch (err) {
    console.error("Error:", err);
    res.status(500).render("../views/shared/_Document", { errorMessage: "Internal Server Error", pageContent: "error404" });
  }
}


// Set up Multer for handling file uploads
const storage = multer.diskStorage({
  destination: function (req, file, cb) {
      cb(null, 'uploads/images'); // Save uploaded files to the 'uploads/images' directory
  },
  filename: function (req, file, cb) {
      // Generate unique filename by appending current timestamp
      cb(null, file.fieldname + '-' + Date.now() + path.extname(file.originalname));
  }
});

const upload = multer({ storage: storage });


// Function to handle image upload
const uploadImage = (req, res) => {
  upload.single('image')(req, res, function(err) {
      if (err instanceof multer.MulterError) {
          // Multer error occurred
          return res.status(400).json({ error: 'Error uploading image' });
      } else if (err) {
          // Other error occurred
          return res.status(500).json({ error: 'Internal Server Error' });
      }

      // Image uploaded successfully
      const imagePath = '/uploads/images/' + req.file.filename;
      res.status(200).json({ imagePath: imagePath });
  });
};


// //#region Image upload
// const storeImage = multer.diskStorage({
//   destination: function (req, file, cb) {
//     const uploadDir = "uploads/images/";
    
//     if (!fs.existsSync(uploadDir)) {
//       fs.mkdirSync(uploadDir, { recursive: true });
//     }
//     cb(null, uploadDir); 
//   },
//   filename: function (req, file, cb) {
//     cb(
//       null,
//       file.fieldname + "-" + Date.now() + path.extname(file.originalname)
//     );
//   },
// });


// // // Initialize multer upload for images
// const uploadImage = multer({
//   storage: storeImage,
//   limits: { fileSize: 1000000 }, // 1 MB file size limit
//   fileFilter: function (req, file, cb) {
//     checkFileType(file, cb);
//   },
// }).fields([
//   { name: 'productimage1', maxCount: 1 },
//   { name: 'productimage2', maxCount: 1 }, 
//   { name: 'productimage3', maxCount: 1 }, 
//   { name: 'productimage5', maxCount: 1 }, 
// ]);


// // Check file type
// function checkFileType(file, cb) {
//   const filetypes = /jpeg|jpg|png|gif/;
//   const extname = filetypes.test(path.extname(file.originalname).toLowerCase());
//   const mimetype = filetypes.test(file.mimetype);

//   if (mimetype && extname) {
//     return cb(null, true);
//   } else {
//     cb("Error: Only images are allowed!");
//   }
// }

// //#endregion



module.exports = {
    SalesmenKYC : SalesmenKYC,
    UploadDocument : UploadDocument,
    ParentID: ParentID,
    PersonalDetail : PersonalDetail,
    uploadImage: uploadImage
    
}