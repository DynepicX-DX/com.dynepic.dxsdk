// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
using System;
using System.Collections;
using System.Collections.Generic;
namespace SketchfabAPI
{
    public class Tag
    {
        public string name { get; set; }
        public string slug { get; set; }
        public string uri { get; set; }
    }

    public class Category
    {
        public string name { get; set; }
    }

    public class Image
    {
        public string uid { get; set; }
        public int size { get; set; }
        public int width { get; set; }
        public string url { get; set; }
        public int height { get; set; }
    }

    public class Thumbnails
    {
        public List<Image> images { get; set; }
    }

    public class Avatar
    {
        public string uri { get; set; }
        public List<Image> images { get; set; }
    }

    public class User
    {
        public string uid { get; set; }
        public string username { get; set; }
        public string displayName { get; set; }
        public string profileUrl { get; set; }
        public string account { get; set; }
        public Avatar avatar { get; set; }
        public string uri { get; set; }
    }

    public class Gltf
    {
        public int? size { get; set; }
    }

    public class Archives
    {
        public Gltf gltf { get; set; }
    }

    public class Model
    {
        public string uri { get; set; }
        public string uid { get; set; }
        public string name { get; set; }
        public DateTime? staffpickedAt { get; set; }
        public int viewCount { get; set; }
        public int likeCount { get; set; }
        public int animationCount { get; set; }
        public string viewerUrl { get; set; }
        public string embedUrl { get; set; }
        public int commentCount { get; set; }
        public bool isDownloadable { get; set; }
        public DateTime publishedAt { get; set; }
        public List<Tag> tags { get; set; }
        public List<Category> categories { get; set; }
        public Thumbnails thumbnails { get; set; }
        public User user { get; set; }
        public string description { get; set; }
        public int faceCount { get; set; }
        public DateTime createdAt { get; set; }
        public int vertexCount { get; set; }
        public bool isAgeRestricted { get; set; }
        public Archives archives { get; set; }
    }

    public class User2
    {
        public string uid { get; set; }
        public string uri { get; set; }
        public DateTime dateJoined { get; set; }
        public Avatar avatar { get; set; }
        public string username { get; set; }
        public string displayName { get; set; }
        public string profileUrl { get; set; }
        public string followersUrl { get; set; }
        public string followingsUrl { get; set; }
        public string modelsUrl { get; set; }
        public string collectionsUrl { get; set; }
        public string likesUrl { get; set; }
        public string account { get; set; }
        public int followerCount { get; set; }
        public int followingCount { get; set; }
        public int modelCount { get; set; }
        public int likeCount { get; set; }
        public int collectionCount { get; set; }
        public int subscriptionCount { get; set; }
        public List<object> skills { get; set; }
        public string biography { get; set; }
        public string tagline { get; set; }
        public string website { get; set; }
        public object twitterUsername { get; set; }
        public object facebookUsername { get; set; }
        public object linkedinUsername { get; set; }
        public object city { get; set; }
        public object country { get; set; }
    }

    public class Collection
    {
        public string uri { get; set; }
        public string name { get; set; }
        public string uid { get; set; }
        public string slug { get; set; }
        public string description { get; set; }
        public int modelCount { get; set; }
        public User user { get; set; }
        public string embedUrl { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string models { get; set; }
        public int subscriberCount { get; set; }
        public bool hasRestrictedContent { get; set; }
        public bool isAgeRestricted { get; set; }
    }

    public class Results
    {
        public List<Model> models { get; set; }
        public List<User> users { get; set; }
        public List<Collection> collections { get; set; }
    }

    public class Root
    {
        public Results results { get; set; }
    }



}
