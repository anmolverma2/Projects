
const path = require("path");
const multer = require('multer');

// const storage = multer.diskStorage({
//     destination:function(req,file,cb){
//       cb(null,'uploads/');
//     },
//     filename:function(req,file,cb){
//       const date = new Date();
//       const day = date.getDate();
//       const month = date.toLocaleString('default', { month: 'short' });
//       const year = date.getFullYear();
//       const originalname = file.originalname.replace(/\s+/g, '-'); // Replace spaces with dashes
      
//       const filename = `${day}-${month}-${year}_${originalname}`;
      
//       cb(null, filename);
//     }
//   });
  
  //const imageUpload = multer({storage:storage});




  //#region Image upload
const storeImage = multer.diskStorage({
  destination: function (req, file, cb) {
    const uploadDir = "uploads/images/";
    // Check if the directory exists, if not, create it
    if (!fs.existsSync(uploadDir)) {
      fs.mkdirSync(uploadDir, { recursive: true });
    }
    cb(null, uploadDir); // Set the destination folder for images
  },
  filename: function (req, file, cb) {
    const date = new Date();
      const day = date.getDate();
      const month = date.toLocaleString('default', { month: 'short' });
      const year = date.getFullYear();
      const originalname = file.originalname.replace(/\s+/g, '-'); // Replace spaces with dashes
      
      const filename = `${day}-${month}-${year}_${originalname}`;
    cb(
      null,
      filename
     // file.fieldname + "-" + Date.now() + path.extname(file.originalname)
    );
  },
});

// Initialize multer upload for images
const uploadImages = multer({
  storage: storeImage,
  limits: { fileSize: 1000000 }, // 1 MB file size limit for each image
  fileFilter: function (req, file, cb) {
    checkFileType(file, cb);
  },
}).fields([
  { name: 'bankproof', maxCount: 1 },
  { name: 'panphoto', maxCount: 1 }, 
  { name: 'backproof', maxCount: 1 }, 
  { name: 'frontproof', maxCount: 1 }, 
]);


// Check file type
function checkFileType(file, cb) {
  const filetypes = /jpeg|jpg|png|gif/;
  const extname = filetypes.test(path.extname(file.originalname).toLowerCase());
  const mimetype = filetypes.test(file.mimetype);

  if (mimetype && extname) {
    return cb(null, true);
  } else {
    cb("Error: Only images are allowed!");
  }
}

//#endregion


  module.exports = {
    //imageUpload:imageUpload
    uploadImages: uploadImages
  };