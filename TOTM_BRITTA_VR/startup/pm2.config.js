const homePath = process.env.HOMEPATH;

module.exports = {
  apps : [{
    name   : "TOTM_VR",
    script : homePath + "\\Desktop\\TOTM\\VR\\TOTM_VR_9_2_22\\TOTM_VR.exe",
    restart_delay: 3000,
    env : {
      windowsHide: false
    }
  }]
}

