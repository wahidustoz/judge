export DEBIAN_FRONTEND=noninteractive

# installing build dependencies
buildDeps='software-properties-common git libtool cmake python-dev python3-pip python-pip libseccomp-dev wget curl'
apt-get update && apt-get install -y gnupg ca-certificates tzdata python python3 python-pkg-resources python3-pkg-resources $buildDeps

# installing gcc and g++
add-apt-repository ppa:ubuntu-toolchain-r/test && apt-get update && apt-get install -y gcc-9 g++-9
rm /usr/bin/gcc /usr/bin/g++ && ln -s /usr/bin/gcc-9 /usr/bin/gcc && ln -s /usr/bin/g++-9 /usr/bin/g++

# installing openjdk, golang, mono and nodejs
add-apt-repository ppa:openjdk-r/ppa && add-apt-repository ppa:longsleep/golang-backports 
curl -fsSL https://deb.nodesource.com/setup_14.x | bash -
apt-get update && apt-get install -y golang-go openjdk-8-jdk nodejs mono-complete

# setting server timezone
ln -sf /usr/share/zoneinfo/Asia/UTC /etc/localtime
dpkg-reconfigure -f noninteractive tzdata

# build judger process CMAKE and purge build dependencies
cd /tmp && git clone -b newnew  --depth 1 https://github.com/wahid-d/judger-process.git && cd judger-process
mkdir build && cd build && cmake .. && make && make install
apt-get purge -y --auto-remove $buildDeps
apt-get clean && rm -rf /var/lib/apt/lists/*

    # install .NET
wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
apt-get update
apt-get install -y dotnet-sdk-7.0 dotnet-sdk-6.0

# install pascal
apt-get update && apt-get install -y wget
wget https://sourceforge.net/projects/lazarus/files/Lazarus%20Linux%20amd64%20DEB/Lazarus%202.2.2/fpc-laz_3.2.2-210709_amd64.deb
apt install -y ./fpc-laz_3.2.2-210709_amd64.deb && rm -rf fpc-laz_3.2.2-210709_amd64.deb

# preparing judger environment
useradd -u 2000 compiler && useradd -u 2001 runner
mkdir /judger && mkdir /judger/testcases
chown runner /judger/testcases
mkdir /judger/testcases/0bd41375-8535-473c-b80c-1d59b06bacda
echo "1 2" > /judger/testcases/0bd41375-8535-473c-b80c-1d59b06bacda/1.in
echo "3" > /judger/testcases/0bd41375-8535-473c-b80c-1d59b06bacda/1.out

# prepare dotnet solutions
mkdir /judger/dotnet && cd /judger/dotnet
dotnet new console -o net6 -f net6.0
dotnet new console -o net7 -f net7.0

# clean tmp folder
rm -rf /tmp/*