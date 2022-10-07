const userName = process.env.USERNAME;

module.exports = {
  apps : [{
    name   : "TOTM_VR",
    script : "C:\\Users\\" + userName + "\\Desktop\\TOTM\\VR\\TOTM_VR_9_2_22\\TOTM_VR.exe",
    restart_delay: 3000,
    env : {
      windowsHide: false
    }
  },
  {
    name : "oculus_babysitter",
    script: "C:\\Users\\" + userName + "\\Desktop\\TOTM\\VR\\totm_python\\oculus_babysitter.py",
    restart_delay: 1000
  }]
}
