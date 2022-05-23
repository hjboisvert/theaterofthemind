const userName = process.env.USERNAME;

module.exports = {
  apps : [{
    name   : "TOTM_VR",
    script : "C:\\Users\\" + userName + "\\Desktop\\TOTM\\VR\\build\\TOTM_VR.exe",
    env : {
      windowsHide: false
    }
  }]
}
