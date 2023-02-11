using CNSUniCore.UniInfos;
using Newtonsoft.Json.Linq;

namespace CNSUniCore;
public class ServerInfo
{
    public int ID { get; set; }

    public string Name { get; set; }

    public string IP { get; set; }

    public int Port { get; set; }

    public string RestUser { get; set; }

    public string RestPwd { get; set; }

    public string Token { get; set; }

    public ServerInfo()
    {
        this.ID = 1;
        this.Name = "实例服务器";
        this.IP = "127.0.0.1";
        this.Port = 7878;
        this.RestUser = "豆沙666";
        this.RestPwd = "dousha666";
        this.Token = "";
    }

    public JObject? BanUser(string name, string ip, string uuid, string reason, string time)
    {
        return this.TestToken()
            ? this.GetHttp($"{this.GetUrl()}/uniban/add?ip={ip}&uuid={uuid}&name={name}&reason={reason}&time={time}&token={this.Token}")
            : null;
    }

    public JObject? AddVip(SponsorInfo sponsorInfo)
    {
        return this.TestToken()
            ? this.GetHttp($"{this.GetUrl()}/univip/add?name={sponsorInfo.name}&group={sponsorInfo.group}&origin={sponsorInfo.originGroup}&start={sponsorInfo.startTime}&end={sponsorInfo.endTime}&token={this.Token}")
            : null;
    }

    public JObject? DelVip(string name)
    {
        return this.TestToken() ? this.GetHttp($"{this.GetUrl()}/univip/del?name={name}&token={this.Token}") : null;
    }

    public JObject? DelUser(string name)
    {
        return this.TestToken() ? this.GetHttp($"{this.GetUrl()}/uniban/del?name={name}&token={this.Token}") : null;
    }

    public string GetUrl(bool https = false)
    {
        return https ? $"https://{this.IP}:{this.Port}" : $"http://{this.IP}:{this.Port}";
    }

    public bool TestToken()
    {
        return this.GetHttp($"{this.GetUrl()}/tokentest?token={this.Token}")["status"].ToString() == "200";
    }

    public void CreateToken()
    {
        if (string.IsNullOrEmpty(this.Token))
        {
            this.Token = this.GetHttp($"{this.GetUrl()}/v2/token/create?username={this.RestUser}&password={this.RestPwd}")["token"].ToString();
        }
    }

    public JObject GetHttp(string url)
    {
        return JObject.Parse(new HttpClient().GetAsync(url).Result.Content.ReadAsStringAsync().Result);
    }

    public JObject? RawCommand(string cmd)
    {
        return this.TestToken() ? this.GetHttp($"{this.GetUrl()}/v3/server/rawcmd?cmd={cmd}&token={this.Token}") : null;
    }

    public JObject? AddUser(UserInfo info)
    {
        return this.TestToken()
            ? this.GetHttp($"{this.GetUrl()}/unireg/add?id&={info.ID}name={info.Name}&lastaccessed={info.LastAccessed}&ip={info.KnownIPs}&group={info.UserGroup}&password={info.Password}&uuid={info.UUID}&registered={info.Registered}&token={this.Token}")
            : null;
    }

    public JObject? UpdateUser(UserInfo info)
    {
        return this.TestToken()
            ? this.GetHttp(this.GetUrl() + "/unireg/update?name=" + info.Name + "&uuid=" + info.UUID + "&group=" + info.UserGroup + "&registered=" + info.Registered + "&lastaccessed=" + info.LastAccessed + "&ip=" + info.KnownIPs + "&token=" + this.Token)
            : null;
    }
}